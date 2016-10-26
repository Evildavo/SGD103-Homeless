﻿using UnityEngine;
using System.Collections.Generic;

public class MotelDiner : MonoBehaviour
{
    const float MENU_UPDATE_INTERVAL_HOURS = 0.5f;
    int dayLastRentedRoom;
    float hourAtLastUpdate;
    bool mainMenuOpen;

    public Main Main;
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public JobLocation JobLocation;
    public EatAtDinerEvent EatAtDiner;

    public float RoomCostPerNight;
    public float SleepQualityFactor = 1.0f;

    [Space(10.0f)]
    public bool RoomRented = false;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();

        // Sleep option.
        if (RoomRented)
        {
            options.Add(new Menu.Option(sleepRoom, "Sleep in room"));
        }
        else
        {
            options.Add(new Menu.Option(rentRoom, "Rent room for tonight", RoomCostPerNight, Main.PlayerState.CanAfford(RoomCostPerNight)));
        }

        // Meal options.
        if (EatAtDiner.IsOpen)
        {
            if (Main.GameTime.TimeOfDayHours < 11)
            {
                options.Add(new Menu.Option(buyFood, "Buy breakfast",
                            EatAtDiner.MealCost, Main.PlayerState.CanAfford(EatAtDiner.MealCost)));
            }
            else if (Main.GameTime.TimeOfDayHours > 17)
            {
                options.Add(new Menu.Option(buyFood, "Buy dinner",
                            EatAtDiner.MealCost, Main.PlayerState.CanAfford(EatAtDiner.MealCost)));
            }
            else
            {
                options.Add(new Menu.Option(buyFood, "Buy lunch",
                            EatAtDiner.MealCost, Main.PlayerState.CanAfford(EatAtDiner.MealCost)));
            }
        }

        // Job options.
        if (EatAtDiner.IsOpen)
        {
            if (JobLocation.IsJobAvailableToday)
            {
                options.Add(new Menu.Option(ApplyForJob, "Apply for job"));
            }
            if (JobLocation.PlayerHasJobHere)
            {
                // Note that this is a ghost option to inform the player, not to be a useable menu item.
                string message = "Work (" + JobLocation.GetWorkTimeSummaryShort() + ")";
                options.Add(new Menu.Option(null, message, 0, false));
            }
        }

        options.Add(new Menu.Option(Reset, "Exit"));
        Main.Menu.Show(options);
        mainMenuOpen = true;
    }

    public void ApplyForJob()
    {
        JobLocation.ApplyForJob();
        OpenMainMenu();
    }

    void rentRoom()
    {
        if (!RoomRented ||
            Main.GameTime.Day != dayLastRentedRoom)
        {
            // Pay cost.
            Main.PlayerState.Money -= RoomCostPerNight;

            RoomRented = true;
            dayLastRentedRoom = Main.GameTime.Day;
        }
        OpenMainMenu();
    }

    void sleepRoom()
    {
        Main.SleepManager.Sleep(null, false, SleepQualityFactor);
        mainMenuOpen = false;
    }

    void buyFood()
    {
        // Pay cost.
        Main.PlayerState.Money -= EatAtDiner.MealCost;

        EatAtDiner.Attend();
        mainMenuOpen = false;
    }

    void Reset()
    {
        mainMenuOpen = false;
        Main.Menu.Hide();
        Main.MessageBox.ShowNext();
        if (Trigger)
        {
            Trigger.Reset(Trigger.IsEnabled);
        }
    }
    
    public void OnTrigger()
    {
        JobLocation.CheckForJob(true);
        OpenMainMenu();
    }
    
    public void OnTriggerUpdate()
    {
        // Show warning that job is about to start.
        if (JobLocation.CanWorkNow)
        {
            Reset();
            Main.MessageBox.ShowForTime("Work is about to start.", 2.0f, gameObject);
        }
    }

    public void Update()
    {
        // Update the menu periodically.
        if (mainMenuOpen && Time.time - hourAtLastUpdate > MENU_UPDATE_INTERVAL_HOURS)
        {
            hourAtLastUpdate = Time.time;
            OpenMainMenu();
        }

        // Switch to the job trigger when it's time to start work.
        if (JobLocation.CanWorkNow)
        {
            Trigger.IsEnabled = false;
            JobTrigger.IsEnabled = true;
        }
        else
        {
            Trigger.IsEnabled = true;
            JobTrigger.IsEnabled = false;
        }
    }
}
