﻿using UnityEngine;
using System.Collections.Generic;

public class Hostel : MonoBehaviour {
    bool hasStartedApplying;
    float dayLastChecked;
    float startedApplyingAtHour;
    bool isApplying;
    bool hasCheckedToday;
    bool hasFinishedApplying;
    bool inBusinessHours;
    bool menuIsOpen;

    public Main Main;
    public Trigger Trigger;
    public WashClothesEvent WashClothesEvent;

    public float ApplyingTimeHours;
    public float TimeCostForCheckingHousingApplication;
    public float ChanceApplicationSuccessful;
    public float MoraleRewardForSuccess;
    public float SleepQualityFactor;
    [Range(0.0f, 24.0f)]
    public float CanCheckOrApplyFromHour = 0.0f;
    [Range(0.0f, 24.0f)]
    public float CanCheckOrApplyToHour = 24.0f;

    [Space(10.0f)]
    public bool PlayerHasRoom;

    public void OpenMainMenu()
    {
        menuIsOpen = true;
        List<Menu.Option> options = new List<Menu.Option>();
        if (!PlayerHasRoom)
        {
            if (!inBusinessHours)
            {
                options.Add(new Menu.Option(ApplyForHousing, "The hostel office is closed", 0.0f, false));
            }
            else if (!hasFinishedApplying)
            {
                options.Add(new Menu.Option(ApplyForHousing, "Apply for housing", 0.0f));
            }
            else if (hasFinishedApplying)
            {
                options.Add(new Menu.Option(CheckHousingApplicationSelected, 
                    "Check housing application", 0.0f, !hasCheckedToday));
            }
            options.Add(new Menu.Option(OnExit, "Exit"));

            Main.Menu.Show(options);
        }
        else
        {
            OpenRoomMenu();
        }
    }

    public void OpenRoomMenu()
    {
        Main.UI.ReturnTo = OpenRoomMenu;
        Main.MessageBox.ShowNext();

        menuIsOpen = true;
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(SleepInRoom, "Sleep"));
        options.Add(new Menu.Option(WashClothes, "Wash clothes"));
        options.Add(new Menu.Option(OnExit, "Exit"));
        
        // Allow player to use their inventory.
        Main.PlayerState.IsInPrivate = true;
        Main.Inventory.Show();

        Main.Menu.Show(options);
    }

    void CheckHousingApplicationSelected()
    {
        Main.GameTime.SpendTime(TimeCostForCheckingHousingApplication);
        CheckHousingApplication();
    }

    public void CheckHousingApplication()
    {
        hasCheckedToday = true;
        dayLastChecked = Main.GameTime.Day;

        // Check if the application is successful.
        if (Random.Range(0.0f, 1.0f) < ChanceApplicationSuccessful)
        {
            PlayerHasRoom = true;
            Main.PlayerState.ChangeMorale(MoraleRewardForSuccess);
            Main.MessageBox.ShowForTime("Application successful. You can sleep here from now on.", null, gameObject);
            OpenMainMenu();
        }
        else
        {
            Main.MessageBox.ShowForTime("Nothing is available today. Check back again tomorrow.", null, gameObject);
            reset();
        }
    }

    public void SleepInRoom()
    {
        Main.SleepManager.Sleep(null, false, SleepQualityFactor);
        menuIsOpen = false;
        reset();
    }

    public void WashClothes()
    {
        WashClothesEvent.Attend();
        menuIsOpen = false;
    }

    public void ApplyForHousing()
    {
        if (!hasStartedApplying || dayLastChecked != Main.GameTime.Day)
        {
            // Start applying for job.
            hasStartedApplying = true;
            isApplying = true;
            dayLastChecked = Main.GameTime.Day;
            startedApplyingAtHour = Main.GameTime.TimeOfDayHours;
            Main.Menu.Hide();
            Main.GameTime.AccelerateTime();
            Main.MessageBox.Show("Applying for housing...", gameObject);
        }
    }

    void Start ()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(reset);
    }

    void Update()
    {
        // Check if a day has ticked over so we can check again.
        if (hasFinishedApplying && hasCheckedToday && Main.GameTime.Day != dayLastChecked)
        {
            hasCheckedToday = false;
        }

        // Determine if open today.
        if (Main.GameTime.TimeOfDayHours > CanCheckOrApplyFromHour &&
            Main.GameTime.TimeOfDayHours < CanCheckOrApplyToHour)
        {
            if (!inBusinessHours)
            {
                inBusinessHours = true;
                if (menuIsOpen)
                {
                    OpenMainMenu();
                }
            }
        }
        else if (inBusinessHours)
        {
            inBusinessHours = false;
            if (menuIsOpen)
            {
                OpenMainMenu();
            }
        }
    }
	
	void OnTrigger () {
        OpenMainMenu();
    }

    void OnTriggerUpdate()
    {
        // Stop applying after a certain amount of time.
        if (isApplying && Main.GameTime.TimeOfDayHours - startedApplyingAtHour > ApplyingTimeHours)
        {
            Main.GameTime.ResetToNormalTime();
            isApplying = false;
            hasCheckedToday = true;
            hasFinishedApplying = true;

            CheckHousingApplication();
            reset();
        }
    }

    void OnExit()
    {
        reset();
        Main.MessageBox.ShowNext();
    }

    void reset()
    {
        Main.Menu.Hide();
        Main.Inventory.Hide();
        Trigger.Reset();
        menuIsOpen = false;
    }

}
