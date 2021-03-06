﻿using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private bool isReading;
    private bool isJobSearching;
    private bool isHouseSearching;
    private float hourAtLastCheck;
    private bool hasWarnedAboutClosing = false;
    private bool jobSearchedToday = false;
    private bool houseFound = false;
    private int dayOfLastJobSearch;
    private bool paidForHouse;

    public Main Main;
    public Trigger Trigger;
    public ResumeItem ResumeItem;
    public List<JobLocation> JobLocations;
    public AudioClip Ambience;
    public Sprite Splash;
    public SonPhotoItem SonPhotoItem;

    public float MoraleGainedPerHourReading = 0.05f;
    public float JobSearchTimeHours = 1.0f;
    public float HouseSearchTimeHours = 1.0f;
    public float MoraleGainedForFindingJob;
    public float MoraleGainedForFindingHouse;
    public float HouseBondCost;

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

        // Hide splash screen.
        Main.Splash.Hide();

        // Stop the ambience.
        var audio = GetComponent<AudioSource>();
        audio.Stop();
        audio.clip = null;

        // Resume street audio.
        if (Main.Ambience)
        {
            Main.Ambience.Resume();
        }
        
        Main.Menu.Hide();
        Main.MessageBox.ShowNext();
        Main.GameTime.ResetToNormalTime();
        if (Trigger)
        {
            Trigger.Reset();
        }
    }

    // Creates and returns a new main menu.
    List<Menu.Option> getMainMenu()
    {
        // Show splash screen.
        Main.Splash.Show(Splash);

        // Play ambience audio.
        var audio = GetComponent<AudioSource>();
        if (audio.clip != Ambience)
        {
            audio.clip = Ambience;
            audio.time = 0.0f;
            audio.loop = true;
            audio.Play();
        }

        // Stop street audio.
        if (Main.Ambience)
        {
            Main.Ambience.Pause();
        }

        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnJobSearch, "Job search", 0, !jobSearchedToday));
        options.Add(new Menu.Option(OnReadBook, "Read book"));
        if (!houseFound)
        {
            options.Add(new Menu.Option(OnHouseSearch, "Apartment search"));
        }
        else if (!paidForHouse)
        {
            options.Add(new Menu.Option(OnHouseApply, "Apply for apartment", HouseBondCost, Main.PlayerState.CanAfford(HouseBondCost)));
        }
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
    
    public void OnHouseSearch()
    {
        Main.MessageBox.Show("Searching for an apartment...", gameObject);
        Main.Menu.Hide();
        Main.GameTime.AccelerateTime();
        isHouseSearching = true;
        dayOfLastJobSearch = Main.GameTime.Day;
        hourAtLastCheck = Main.GameTime.TimeOfDayHours;
    }

    public void OnHouseApply()
    {
        if (!Main.PlayerState.HasJob())
        {
            Main.MessageBox.Show("You need to be working to apply for the apartment", gameObject);
        }
        else
        {
            SonPhotoItem.ShowPhoto();
            Main.MessageBox.Show("Congratulations! You're one step closer to getting your son back", gameObject);
            Main.PlayerState.Money -= HouseBondCost;
            paidForHouse = true;
        }
        Main.Menu.Show(getMainMenu());
    }

    void jobSearch()
    {
        Main.MessageBox.Show("Searching for jobs...", gameObject);
        Main.Menu.Hide();
        Main.GameTime.AccelerateTime();
        isJobSearching = true;
        dayOfLastJobSearch = Main.GameTime.Day;
        hourAtLastCheck = Main.GameTime.TimeOfDayHours;
    }

    public void OnReadBook()
    {
        // Choose a random book to read.
        int random = Random.Range(0, Books.Count);

        Main.MessageBox.Show("You are reading \"" + Books[random] + "\"", gameObject);
        Main.Menu.Show(getReadingMenu());
        Main.GameTime.AccelerateTime();
        isReading = true;
    }
    
    public void OnStopReading()
    {
        Main.Menu.Show(getMainMenu());
        Main.MessageBox.ShowNext();
        Main.GameTime.ResetToNormalTime();
        isReading = false;
    }

    public void OnTrigger()
    {
        isJobSearching = false;
        isHouseSearching = false;
        isReading = false;
        Main.Menu.Show(getMainMenu());
    }
    
    public void OnTriggerUpdate()
    {
        // If the library closes exit.
        if (!Trigger.IsInActiveHour)
        {
            Reset();
            Main.MessageBox.ShowQueued("Library has closed.", null, gameObject);
        }

        // Show warning when the library is about to close.
        else
        {
            const float CLOSE_WARNING_GAME_HOURS = 1.0f;
            if (!hasWarnedAboutClosing &&
                Mathf.Abs(Main.GameTime.TimeOfDayHours - Trigger.ActiveToHour) <= CLOSE_WARNING_GAME_HOURS)
            {
                Main.MessageBox.ShowForTime("Library will be closing in 1 hour", null, gameObject, true);
                hasWarnedAboutClosing = true;
            }
        }
        
        // Increase morale while reading.
        if (isReading)
        {
            Main.PlayerState.ChangeMorale(MoraleGainedPerHourReading * Main.GameTime.GameTimeDelta);
        }

        // Search for house after a minimum amount of time.
        if (isHouseSearching && Main.GameTime.TimeOfDayHours - hourAtLastCheck > HouseSearchTimeHours)
        {
            Main.MessageBox.Show("A modest apartment is available for a bond of $" + HouseBondCost.ToString("f2"), gameObject);
            Main.PlayerState.ChangeMorale(MoraleGainedForFindingHouse);
            houseFound = true;
            Main.Menu.Show(getMainMenu());
            Main.GameTime.ResetToNormalTime();
            isHouseSearching = false;
        }
        
        // Search for jobs after a minimum amount of time.
        if (isJobSearching && Main.GameTime.TimeOfDayHours - hourAtLastCheck > JobSearchTimeHours)
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
                    Main.PlayerState.ChangeMorale(MoraleGainedForFindingJob);

                    jobAvailable = true;
                    Main.Menu.Show(getMainMenu());
                    break;
                }
            }

            // Stop searching.
            Main.GameTime.ResetToNormalTime();
            isJobSearching = false;
            jobSearchedToday = true;

            // Report no jobs found.
            if (!jobAvailable)
            {
                Main.MessageBox.ShowForTime("You didn't find any jobs today.", null, gameObject);
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
