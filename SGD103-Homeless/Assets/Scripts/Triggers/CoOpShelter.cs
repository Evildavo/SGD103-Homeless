using UnityEngine;
using System.Collections.Generic;

public class CoOpShelter : MonoBehaviour {
    private bool inMainMenu = false;
    private bool wasSoupKitchenOpen = false;
    private bool wasCounsellingOpen = false;
    private bool wasAddictionSupportOpen = false;
    private float activeFromHour;
    private float activeToHour;
    
    public Main Main;
    public Trigger Trigger;
    public EventAtLocation SoupKitchenEvent;
    public EventAtLocation CounsellingEvent;
    public EventAtLocation AddictionSupportEvent;

    public float TimeCostToReadNotice;

	void Start ()
    {
        activeFromHour = Trigger.ActiveFromHour;
        activeToHour = Trigger.ActiveToHour;

        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        Trigger.RegisterOnCloseRequested(reset);
    }

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        /*options.Add(new Menu.Option(OpenCoOpShopMenu, "Co-op shop"));*/
        options.Add(new Menu.Option(ReadNoticeBoard, "Read notice board"));
        /*options.Add(new Menu.Option(RequestEmergencyAccomodation, "Request emergency accommodation"));*/
        if (SoupKitchenEvent && SoupKitchenEvent.IsOpen)
        {
            options.Add(new Menu.Option(AttendSoupKitchen, "Attend soup kitchen"));
        }
        if (CounsellingEvent && CounsellingEvent.IsOpen)
        {
            options.Add(new Menu.Option(AttendCounselling, "Attend counselling"));
        }
        if (AddictionSupportEvent && AddictionSupportEvent.IsOpen)
        {
            options.Add(new Menu.Option(AttendAddictionSupport, "Attend addiction support therapy"));
        }
        options.Add(new Menu.Option(reset, "Exit"));

        Main.Menu.Show(options);
        inMainMenu = true;
    }

    public void OpenCoOpShopMenu()
    {
        Main.MessageBox.ShowNext();
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenMainMenu, "Exit"));

        Main.Menu.Show(options);
        inMainMenu = false;
    }

    public void ReadNoticeBoard()
    {
        var GameTime = Main.GameTime;

        // Apply time cost.
        GameTime.SpendTime(TimeCostToReadNotice);

        string message = "";
        if (SoupKitchenEvent)
        {
            message +=
                "Soup Kitchen: " +
                GameTime.DayOfTheWeekAsShortString(SoupKitchenEvent.Day) + " " +
                GameTime.GetTimeAsString(SoupKitchenEvent.FromHour) + " to " +
                GameTime.GetTimeAsString(SoupKitchenEvent.ToHour) + "\n";
        }
        if (CounsellingEvent)
        {
            message += "Counselling: " +
                GameTime.DayOfTheWeekAsShortString(CounsellingEvent.Day) + " " +
                GameTime.GetTimeAsString(CounsellingEvent.FromHour) + " to " +
                GameTime.GetTimeAsString(CounsellingEvent.ToHour) + "\n";
        }
        if (AddictionSupportEvent)
        {
            message += "Addiction Support: " +
                GameTime.DayOfTheWeekAsShortString(AddictionSupportEvent.Day) + " " +
                GameTime.GetTimeAsString(AddictionSupportEvent.FromHour) + " to " +
                GameTime.GetTimeAsString(AddictionSupportEvent.ToHour);
        }
        Main.MessageBox.Show(message, gameObject);
        OpenMainMenu();
    }

    public void RequestEmergencyAccomodation()
    {
        Debug.Log("Emergency accomodation requested");
        OpenMainMenu();
    }

    public void AttendSoupKitchen()
    {
        SoupKitchenEvent.Attend();
        Main.Menu.Hide();
    }

    public void AttendCounselling()
    {
        CounsellingEvent.Attend();
        Main.Menu.Hide();
    }

    public void AttendAddictionSupport()
    {
        AddictionSupportEvent.Attend();
        Main.Menu.Hide();
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
        if (!wasSoupKitchenOpen && SoupKitchenEvent && SoupKitchenEvent.IsOpen)
        {
            Main.MessageBox.ShowForTime("The soup kitchen has opened for today", 4.0f, gameObject);
            wasSoupKitchenOpen = true;
            serviceOpened = true;
        }
        else if (wasSoupKitchenOpen && SoupKitchenEvent && !SoupKitchenEvent.IsOpen)
        {
            Main.MessageBox.ShowQueued("The soup kitchen has closed for today", 4.0f, gameObject);
            wasSoupKitchenOpen = false;
            serviceClosed = true;
        }
        if (!wasCounsellingOpen && CounsellingEvent && CounsellingEvent.IsOpen)
        {
            Main.MessageBox.ShowForTime("Counselling services have opened for today", 4.0f, gameObject);
            wasCounsellingOpen = true;
            serviceOpened = true;
        }
        else if (wasCounsellingOpen && CounsellingEvent && !CounsellingEvent.IsOpen)
        {
            Main.MessageBox.ShowQueued("Counselling services have closed for today", 4.0f, gameObject);
            wasCounsellingOpen = false;
            serviceClosed = true;
        }
        if (!wasAddictionSupportOpen && AddictionSupportEvent && AddictionSupportEvent.IsOpen)
        {
            Main.MessageBox.ShowForTime("Addiction support therapy has opened for tonight", 4.0f, gameObject);
            wasAddictionSupportOpen = true;
            serviceOpened = true;
        }
        else if (wasAddictionSupportOpen && AddictionSupportEvent && !AddictionSupportEvent.IsOpen)
        {
            Main.MessageBox.ShowQueued("Addiction support therapy has closed for tonight", 4.0f, gameObject);
            wasAddictionSupportOpen = false;
            serviceClosed = true;
        }

        // Update the menu.
        if ((serviceOpened || serviceClosed) && inMainMenu)
        {
            OpenMainMenu();
        }
    }

    public void OnPlayerExit()
    {
        if (SoupKitchenEvent.IsCurrentlyAttending)
        {
            SoupKitchenEvent.Leave();
        }
        if (CounsellingEvent.IsCurrentlyAttending)
        {
            CounsellingEvent.Leave();
        }
        if (AddictionSupportEvent.IsCurrentlyAttending)
        {
            AddictionSupportEvent.Leave();
        }
    }

    void reset()
    {
        Main.Menu.Hide();
        Main.MessageBox.ShowNext();
        Trigger.Reset();
    }
    
    void Update()
    {
        // Set closing hours differently when an event is on.
        if (SoupKitchenEvent.IsOpen || CounsellingEvent.IsOpen || AddictionSupportEvent.IsOpen)
        {
            Trigger.ActiveFromHour = 0.0f;
            Trigger.ActiveToHour = 24.0f;
        }
        else
        {
            Trigger.ActiveFromHour = activeFromHour;
            Trigger.ActiveToHour = activeToHour;
        }
    }

}
