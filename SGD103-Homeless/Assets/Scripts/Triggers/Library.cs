using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private bool isReading;
    private bool isJobSearching;
    private float timeAtLastCheck;
    private bool hasWarnedAboutClosing = false;
    private bool jobSearchedToday = false;
    private int dayOfLastJobSearch;

    public Main Main;
    public Trigger Trigger;
    public ResumeItem ResumeItem;
    public List<JobLocation> JobLocations;

    public float MoraleGainedPerSecondReading = 0.05f;
    public float JobSearchTimeSeconds = 4.0f;

    public List<string> Books;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    // Reset to starting values.
    void Reset()
    {
        isReading = false;
        hasWarnedAboutClosing = false;
        Main.PlayerState.HighlightMorale = false;
        Main.Menu.Hide();
        Main.MessageBox.ShowNext();
        Main.GameTime.TimeScale = Main.GameTime.NormalTimeScale;
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
        options.Add(new Menu.Option(Reset, "Exit"));
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
        Main.MessageBox.Show("Searching for jobs...", gameObject);
        Main.Menu.Hide();
        Main.GameTime.TimeScale = Main.GameTime.AcceleratedTimeScale;
        isJobSearching = true;
        dayOfLastJobSearch = Main.GameTime.Day;
        timeAtLastCheck = Time.time;
    }
    
    public void OnJobSearch()
    {
        // Ask if they still want to search with a full inventory.
        if (Main.Inventory.IsInventoryFull && !Main.Inventory.HasItem(ResumeItem))
        {
            ConfirmationBox.OnChoiceMade onChoice = (bool yes) =>
            {
                if (yes)
                {
                    jobSearch();
                }
            };
            Main.ConfirmationBox.Open(onChoice, 
                "You don't have room in your inventory to take any resumes. Search anyway?", "Yes", "No");
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

        Main.MessageBox.Show("You are reading \"" + Books[random] + "\"", gameObject);
        Main.Menu.Show(getReadingMenu());
        Main.PlayerState.HighlightMorale = true;
        Main.GameTime.TimeScale = Main.GameTime.AcceleratedTimeScale;
        isReading = true;
    }
    
    public void OnStopReading()
    {
        Main.Menu.Show(getMainMenu());
        Main.MessageBox.ShowNext();
        Main.PlayerState.HighlightMorale = false;
        Main.GameTime.TimeScale = Main.GameTime.NormalTimeScale;
        isReading = false;
    }

    public void OnTrigger()
    {
        isJobSearching = false;
        isReading = false;
        Main.Menu.Show(getMainMenu());
    }
    
    public void OnTriggerUpdate()
    {
        // If the library closes exit.
        if (!Trigger.IsInActiveHour)
        {
            Reset();
            Main.MessageBox.ShowQueued("Library has closed.", 2.0f, gameObject);
        }

        // Show warning when the library is about to close.
        else
        {
            const float CLOSE_WARNING_GAME_HOURS = 1.0f;
            if (!hasWarnedAboutClosing &&
                Mathf.Abs(Main.GameTime.TimeOfDayHours - Trigger.ActiveToHour) <= CLOSE_WARNING_GAME_HOURS)
            {
                Main.MessageBox.ShowForTime("Library will be closing in 1 hour", 2.0f, gameObject, true);
                hasWarnedAboutClosing = true;
            }
        }
        
        // Increase morale while reading.
        if (isReading)
        {
            Main.PlayerState.ChangeMorale(MoraleGainedPerSecondReading * Time.deltaTime);
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
                    Main.MessageBox.Show(message, gameObject);

                    jobAvailable = true;
                    Main.Menu.Show(getMainMenu());
                    break;
                }
            }

            // Stop searching.
            Main.GameTime.TimeScale = Main.GameTime.NormalTimeScale;
            isJobSearching = false;
            jobSearchedToday = true;

            // Report no jobs found.
            if (!jobAvailable)
            {
                Main.MessageBox.ShowForTime("You didn't find any jobs today.", 2.0f, gameObject);
                Main.Menu.Show(getMainMenu());
            }

            // Add or update resumes.
            var Inventory = Main.Inventory;
            if (!Inventory.IsInventoryFull)
            {
                if (Inventory.HasItem(ResumeItem))
                {
                    ResumeItem item = Inventory.ItemContainer.GetComponentInChildren<ResumeItem>();
                    item.NumUses = 2;
                    /*item.RelevantToDay = GameTime.Day;*/
                }
                else
                {
                    ResumeItem item = Instantiate(ResumeItem);
                    item.Main = Main;
                    item.NumUses = 2;
                    /*item.RelevantToDay = GameTime.Day;*/
                    Inventory.AddItem(item);
                }
                Inventory.ShowPreview();
            }
        }
    }

    void Update()
    {
        // Check if a day has ticked over so we can job search again.
        if (jobSearchedToday && Main.GameTime.Day != dayOfLastJobSearch)
        {
            jobSearchedToday = false;
        }
    }

}
