using UnityEngine;
using System.Collections.Generic;

public class MotelDiner : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public JobLocation JobLocation;
    public EatAtDinerEvent EatAtDiner;

    public float RoomCostPerNight;
    public float SleepQualityFactor = 1.0f;

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
        options.Add(new Menu.Option(rentRoom, "Rent room for tonight", RoomCostPerNight, Main.PlayerState.CanAfford(RoomCostPerNight)));
        options.Add(new Menu.Option(sleepRoom, "Sleep in room"));

        // Meal options.
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

        // Job options.
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
        options.Add(new Menu.Option(Reset, "Exit"));
        Main.Menu.Show(options);
    }

    public void ApplyForJob()
    {
        JobLocation.ApplyForJob();
        OpenMainMenu();
    }

    void rentRoom()
    {
        Debug.Log("Renting room");
    }

    void sleepRoom()
    {
        Main.SleepManager.Sleep(null, false, SleepQualityFactor);
    }

    void buyFood()
    {
        // Pay cost.
        Main.PlayerState.Money -= EatAtDiner.MealCost;

        EatAtDiner.Attend();
    }

    void Reset()
    {
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
