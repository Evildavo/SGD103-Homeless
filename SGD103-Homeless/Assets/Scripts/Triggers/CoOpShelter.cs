﻿using UnityEngine;
using System.Collections.Generic;

public class CoOpShelter : MonoBehaviour {
    private bool inMainMenu = false;
    private bool wasSoupKitchenOpen = false;
    private bool wasCounsellingOpen = false;
    private bool wasAddictionSupportOpen = false;

    public Main Main;
    public Trigger Trigger;
    public WeeklyEvent SoupKitchenEvent;
    public WeeklyEvent CounsellingEvent;
    public WeeklyEvent AddictionSupportEvent;

    // An event that occurs on a weekly basis.
    [System.Serializable]
    public class WeeklyEvent
    {
        public Main Main;
        public GameTime.DayOfTheWeekEnum[] Days;
        [Header("Note: Does NOT support wrapping (e.g. 11pm to 2am).")]
        public float FromHour;
        public float ToHour;

        // Returns true if the event is currently running.
        public bool IsOpen()
        {
            // Determine if open today.
            bool openToday = false;
            foreach (GameTime.DayOfTheWeekEnum day in Days)
            {
                if (day == Main.GameTime.DayOfTheWeek)
                {
                    openToday = true;
                    break;
                }
            }
            if (openToday)
            {
                // Determine if open now.
                if (Main.GameTime.TimeOfDayHours > FromHour &&
                    Main.GameTime.TimeOfDayHours < ToHour)
                {
                    return true;
                }
            }
            return false;
        }
    }

	void Start () {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(reset);
    }

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenCoOpShopMenu, "Co-op shop"));
        options.Add(new Menu.Option(ReadNoticeBoard, "Read notice board"));
        options.Add(new Menu.Option(RequestEmergencyAccomodation, "Request emergency accommodation"));
        if (SoupKitchenEvent.IsOpen())
        {
            options.Add(new Menu.Option(AttendSoupKitchen, "Attend soup kitchen"));
        }
        if (CounsellingEvent.IsOpen())
        {
            options.Add(new Menu.Option(AttendCounselling, "Attend counselling"));
        }
        if (AddictionSupportEvent.IsOpen())
        {
            options.Add(new Menu.Option(AttendAddictionSupport, "Attend addiction support therapy"));
        }
        options.Add(new Menu.Option(reset, "Exit"));

        Main.Menu.Show(options);
        inMainMenu = true;
    }

    public void OpenCoOpShopMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenMainMenu, "Exit"));

        Main.Menu.Show(options);
        inMainMenu = false;
    }

    public void ReadNoticeBoard()
    {
        Debug.Log("Reading notice board");
        OpenMainMenu();
    }

    public void RequestEmergencyAccomodation()
    {
        Debug.Log("Emergency accomodation requested");
        OpenMainMenu();
    }

    public void AttendSoupKitchen()
    {
        Debug.Log("Soup kitchen attended");
        OpenMainMenu();
    }

    public void AttendCounselling()
    {
        Debug.Log("Counselling attended");
        OpenMainMenu();
    }

    public void AttendAddictionSupport()
    {
        Debug.Log("Addiction support therapy attended");
        OpenMainMenu();
    }

    public void OnTrigger()
    {
        OpenMainMenu();
    }

    public void OnTriggerUpdate()
    {
        // Notify the user that a service has opened or closed.
        bool serviceOpened = false;
        bool serviceClosed = false;
        if (!wasSoupKitchenOpen && SoupKitchenEvent.IsOpen())
        {
            Main.MessageBox.ShowForTime("The soup kitchen has opened for today", 4.0f, gameObject);
            wasSoupKitchenOpen = true;
            serviceOpened = true;
        }
        else if (wasSoupKitchenOpen && !SoupKitchenEvent.IsOpen())
        {
            Main.MessageBox.ShowForTime("The soup kitchen has closed for today", 4.0f, gameObject);
            wasSoupKitchenOpen = false;
            serviceClosed = true;
        }
        if (!wasCounsellingOpen && CounsellingEvent.IsOpen())
        {
            Main.MessageBox.ShowForTime("Counselling services have opened for today", 4.0f, gameObject);
            wasCounsellingOpen = true;
            serviceOpened = true;
        }
        else if (wasCounsellingOpen && !CounsellingEvent.IsOpen())
        {
            Main.MessageBox.ShowForTime("Counselling services have closed for today", 4.0f, gameObject);
            wasCounsellingOpen = true;
            serviceClosed = true;
        }
        if (!wasAddictionSupportOpen && AddictionSupportEvent.IsOpen())
        {
            Main.MessageBox.ShowForTime("Addiction support therapy has opened for today", 4.0f, gameObject);
            wasAddictionSupportOpen = true;
            serviceOpened = true;
        }
        else if (wasAddictionSupportOpen && !AddictionSupportEvent.IsOpen())
        {
            Main.MessageBox.ShowForTime("Addiction support therapy has closed for today", 4.0f, gameObject);
            wasAddictionSupportOpen = true;
            serviceClosed = true;
        }

        // Update the menu.
        if ((serviceOpened || serviceClosed) && inMainMenu)
        {
            OpenMainMenu();
        }
    }

    void reset()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }

}
