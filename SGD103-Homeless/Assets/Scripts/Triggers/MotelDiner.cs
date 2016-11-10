using UnityEngine;
using System.Collections.Generic;

public class MotelDiner : MonoBehaviour
{
    const float MENU_UPDATE_INTERVAL_HOURS = 0.5f;
    int dayLastRentedRoom;
    float hourAtLastUpdate;
    bool mainMenuOpen;
    bool wentStraightToRoom;

    public Main Main;
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public JobLocation JobLocation;
    public EatAtDinerEvent EatAtDiner;
    public WashClothesEvent WashClothesEvent;
    public AudioClip FlushSound;

    public float LeaveRoomByHour = 8.0f;
    public float RoomCostPerNight;
    public float SleepQualityFactor = 1.0f;
    public float TimeCostToRentRoom;

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

        // Rent room option.
        if (!RoomRented && Main.GameTime.TimeOfDayHours >= LeaveRoomByHour)
        {
            options.Add(new Menu.Option(rentRoom, "Rent room for tonight", RoomCostPerNight, Main.PlayerState.CanAfford(RoomCostPerNight)));
        }

        // Diner options.
        if (EatAtDiner.IsOpen)
        {
            wentStraightToRoom = false;

            // Go to room option.
            if (RoomRented)
            {
                options.Add(new Menu.Option(OpenRoomMenu, "Go to room"));
            }

            // Go to toilet option.
            options.Add(new Menu.Option(OpenToiletMenu, "Go to toilet"));
        
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
        }
        else if (RoomRented)
        {
            wentStraightToRoom = true;
            OpenRoomMenu();
            return;
        }

        options.Add(new Menu.Option(Reset, "Exit"));
        Main.Menu.Show(options);
        mainMenuOpen = true;
    }

    public void OpenRoomMenu()
    {
        Main.UI.ReturnTo = OpenRoomMenu;
        Main.MessageBox.ShowNext();
        
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(sleepInRoom, "Sleep"));
        options.Add(new Menu.Option(WashClothes, "Wash clothes"));
        if (wentStraightToRoom)
        {
            options.Add(new Menu.Option(Reset, "Exit"));
        }
        else
        {
            options.Add(new Menu.Option(OpenMainMenu, "Back"));
        }

        // Allow player to use their inventory.
        Main.PlayerState.IsInPrivate = true;
        Main.Inventory.Show();

        Main.Menu.Show(options);
        mainMenuOpen = false;
    }

    public void OpenToiletMenu()
    {
        Main.UI.ReturnTo = OpenToiletMenu;
        Main.MessageBox.ShowNext();

        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onExitToilet, "Back"));

        // Allow player to use their inventory.
        Main.PlayerState.IsInPrivate = true;
        Main.Inventory.Show();

        Main.Menu.Show(options);
        mainMenuOpen = false;
    }

    void onExitToilet()
    {
        // Play toilet flush sound.
        var audio = GetComponent<AudioSource>();
        audio.clip = FlushSound;
        audio.time = 0.0f;
        audio.Play();

        OpenMainMenu();
    }

    public void ApplyForJob()
    {
        JobLocation.ApplyForJob();
        OpenMainMenu();
    }

    public void WashClothes()
    {
        WashClothesEvent.Attend();
        mainMenuOpen = false;
    }

    void rentRoom()
    {
        if (!RoomRented ||
            Main.GameTime.Day != dayLastRentedRoom)
        {
            // Pay cost.
            Main.PlayerState.Money -= RoomCostPerNight;

            // Apply time cost.
            Main.GameTime.SpendTime(TimeCostToRentRoom);

            RoomRented = true;
            dayLastRentedRoom = Main.GameTime.Day;
        }
        OpenMainMenu();
    }

    void sleepInRoom()
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
        Main.Inventory.Hide();
        Main.MessageBox.ShowNext();
        if (Trigger)
        {
            Trigger.Reset(Trigger.IsEnabled);
        }
    }
    
    public void OnTrigger()
    {
        if (EatAtDiner.IsOpen)
        {
            JobLocation.CheckForJob(true);
        }
        OpenMainMenu();
    }
    
    public void OnTriggerUpdate()
    {
        // Show warning that job is about to start.
        if (JobLocation.CanWorkNow)
        {
            Reset();
            Main.MessageBox.ShowForTime("Work is about to start.", null, gameObject);
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

        // Must leave room by the hour rooms become available.
        var GameTime = Main.GameTime;
        if (RoomRented &&
            GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, LeaveRoomByHour).shortest <= 
            GameTime.GameTimeDelta)
        {
            RoomRented = false;

            // Wake up if asleep.
            if (Main.SleepManager.IsAsleep)
            {
                Main.SleepManager.Wake();
            }

            // Ask the player if they want to rent the room again.
            if (Main.PlayerState.CanAfford(RoomCostPerNight))
            {
                ConfirmationBox.OnChoiceMade onChoice = (bool yes) =>
                {
                    if (yes)
                    {
                        rentRoom();
                    }
                };
                Main.ConfirmationBox.Open(onChoice, 
                    "Would you like to rent the room again for today?", "Yes", "No");
            }
            else
            {
                Main.MessageBox.ShowForTime("You no longer have access to the room", null, gameObject);
            }
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
