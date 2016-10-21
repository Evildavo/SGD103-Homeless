using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private bool isReading;
    private bool isJobSearching;
    private float timeAtLastCheck;
    private bool hasWarnedAboutClosing = false;
    private bool jobSearchedToday = false;
    private int dayOfLastJobSearch;

    public Trigger Trigger;
    public Menu Menu;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public PlayerState PlayerState;
    public GameTime GameTime;
    public Inventory Inventory;
    public InventoryItemDescription ItemDescription;
    public ResumeItem ResumeItem;
    public List<JobLocation> JobLocations;

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
        MessageBox.ShowNext();
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
        options.Add(new Menu.Option(OnJobSearch, "Job search", 0, !jobSearchedToday));
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
        jobSearchedToday = true;
        dayOfLastJobSearch = GameTime.Day;
        timeAtLastCheck = Time.time;
    }
    
    public void OnJobSearch()
    {
        // Ask if they still want to search with a full inventory.
        if (Inventory.IsInventoryFull && !Inventory.HasItem(ResumeItem))
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
        MessageBox.ShowNext();
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
            MessageBox.ShowQueued("Library has closed.", 2.0f, gameObject);
        }

        // Show warning when the library is about to close.
        else
        {
            const float CLOSE_WARNING_GAME_HOURS = 1.0f;
            if (!hasWarnedAboutClosing &&
                Mathf.Abs(GameTime.TimeOfDayHours - Trigger.ActiveToHour) <= CLOSE_WARNING_GAME_HOURS)
            {
                MessageBox.ShowForTime("Library will be closing in 1 hour", 2.0f, gameObject, true);
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
            // Randomise the job list.
            List<JobLocation> randomJobList = new List<JobLocation>(JobLocations);
            randomJobList.Sort((a, b) => Random.Range(-1, 2));

            // See if any jobs are available.
            bool jobAvailable = false;
            foreach (JobLocation job in randomJobList)
            {
                // Shows a message about job availability.
                job.CheckForJob(false);
                if (job.IsJobAvailableToday)
                {
                    string message = "A job position is available today as: " + job.Job.Role + 
                                     "\nat " + job.Name + ". Resume updated.";
                    MessageBox.Show(message, gameObject);

                    jobAvailable = true;
                    Menu.Show(getMainMenu());
                    break;
                }
            }

            // Stop searching.
            GameTime.TimeScale = GameTime.NormalTimeScale;
            isJobSearching = false;

            // Report no jobs found.
            if (!jobAvailable)
            {
                MessageBox.ShowForTime("You didn't find any jobs today.", 2.0f, gameObject);
                Menu.Show(getMainMenu());
            }

            // Add or update resumes.
            if (!Inventory.IsInventoryFull)
            {
                if (Inventory.HasItem(ResumeItem))
                {
                    ResumeItem item = Inventory.ItemContainer.GetComponentInChildren<ResumeItem>();
                    item.NumUses = 2;
                    /*item.RelevantToDay = GameTime.Day;*/
                    Inventory.ShowPreview();
                }
                else
                {
                    ResumeItem item = Instantiate(ResumeItem);
                    item.InventoryItemDescription = ItemDescription;
                    item.MessageBox = MessageBox;
                    item.NumUses = 2;
                    /*item.RelevantToDay = GameTime.Day;*/
                    Inventory.AddItem(item);
                }
            }
        }

        // Leave menu on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }

    void Update()
    {
        // Check if a day has ticked over so we can job search again.
        if (jobSearchedToday && GameTime.Day != dayOfLastJobSearch)
        {
            jobSearchedToday = false;
        }
    }

}
