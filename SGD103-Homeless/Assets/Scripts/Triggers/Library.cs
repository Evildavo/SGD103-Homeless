using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private bool isReading;
    private bool isJobSearching;
    private float timeAtLastCheck;
    private bool hasWarnedAboutClosing = false;

    public Trigger Trigger;
    public Menu Menu;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public PlayerState PlayerState;
    public GameTime GameTime;
    public Inventory Inventory;

    public float MoraleGainedPerSecondReading = 0.05f;
    public float JobSearchTimeSeconds = 4.0f;

    public List<string> Books;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    // Reset to starting values.
    void reset()
    {
        isReading = false;
        hasWarnedAboutClosing = false;
        PlayerState.HighlightMorale = false;
        Menu.Hide();
        MessageBox.Hide();
        GameTime.TimeScale = GameTime.NormalTimeScale;
        if (Trigger)
        {
            Trigger.Reset();
        }
    }

    // Creates and returns a new main menu.
    List<Menu.Option> getMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnJobSearch, "Job search"));
        options.Add(new Menu.Option(OnReadBook, "Read book"));
        options.Add(new Menu.Option(OnExit, "Exit"));
        return options;
    }

    // Creates and returns a new reading menu.
    List<Menu.Option> getReadingMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnStopReading, "Stop reading"));
        return options;
    }

    void jobSearch()
    {
        MessageBox.Show("Searching for jobs...", gameObject);
        Menu.Hide();
        GameTime.TimeScale = GameTime.AcceleratedTimeScale;
        isJobSearching = true;
        timeAtLastCheck = Time.time;
    }
    
    public void OnJobSearch()
    {
        // Ask if they still want to search with a full inventory.
        if (Inventory.IsInventoryFull)
        {
            ConfirmationBox.OnChoiceMade onChoice = (bool yes) =>
            {
                if (yes)
                {
                    jobSearch();
                }
            };
            ConfirmationBox.Open(onChoice, "You don't have room in your inventory to take any resumes. Search anyway?", "Yes", "No");
        }
        else
        {
            jobSearch();
        }
    }

    public void OnReadBook()
    {
        // Choose a random book to read.
        int random = Random.Range(0, Books.Count);

        MessageBox.Show("You are reading \"" + Books[random] + "\"", gameObject);
        Menu.Show(getReadingMenu());
        PlayerState.HighlightMorale = true;
        GameTime.TimeScale = GameTime.AcceleratedTimeScale;
        isReading = true;
    }

    public void OnExit()
    {
        reset();
    }

    public void OnStopReading()
    {
        Menu.Show(getMainMenu());
        MessageBox.Hide();
        PlayerState.HighlightMorale = false;
        GameTime.TimeScale = GameTime.NormalTimeScale;
        isReading = false;
    }

    public void OnTrigger()
    {
        isJobSearching = false;
        isReading = false;
        Menu.Show(getMainMenu());
    }

    public void OnPlayerExit()
    {
        reset();
    }

    public void OnTriggerUpdate()
    {
        // If the library closes exit.
        if (!Trigger.IsInActiveHour)
        {
            reset();
            MessageBox.ShowForTime("Library has closed.", 2.0f, gameObject);
        }

        // Show warning when the library is about to close.
        else
        {
            const float CLOSE_WARNING_GAME_HOURS = 1.0f;
            if (!hasWarnedAboutClosing &&
                Mathf.Abs(GameTime.TimeOfDayHours - Trigger.ActiveToHour) <= CLOSE_WARNING_GAME_HOURS)
            {
                MessageBox.ShowForTime("Library will be closing in 1 hour", 2.0f, gameObject);
                hasWarnedAboutClosing = true;
            }
        }
        
        // Increase morale while reading.
        if (isReading)
        {
            PlayerState.Morale += MoraleGainedPerSecondReading * Time.deltaTime;
        }
        
        // Search for jobs after a minimum amount of time.
        if (isJobSearching && Time.time - timeAtLastCheck > JobSearchTimeSeconds)
        {
            MessageBox.ShowForTime("You didn't find any jobs today.", 2.0f, gameObject);
            GameTime.TimeScale = GameTime.NormalTimeScale;
            Menu.Show(getMainMenu());
        }

        // Leave menu on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }

}
