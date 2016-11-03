using UnityEngine;
using System.Collections.Generic;

public class Hostel : MonoBehaviour {
    bool hasStartedApplying;
    float dayLastChecked;
    float startedApplyingAtHour;
    bool isApplying;
    bool hasCheckedToday;
    bool hasFinishedApplying;
    
    public Main Main;
    public Trigger Trigger;

    public float ApplyingTimeHours;
    public float TimeCostForCheckingHousingApplication;

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        if (!hasFinishedApplying)
        {
            options.Add(new Menu.Option(ApplyForHousing, "Apply for housing"));
        }
        else if (hasFinishedApplying)
        {
            options.Add(new Menu.Option(CheckHousingApplicationSelected, "Check housing application", 0.0f, !hasCheckedToday));
        }
        options.Add(new Menu.Option(OnExit, "Exit"));

        Main.Menu.Show(options);
    }

    void CheckHousingApplicationSelected()
    {
        Main.GameTime.SpendTime(TimeCostForCheckingHousingApplication);
        CheckHousingApplication();
        reset();
    }

    public void CheckHousingApplication()
    {
        hasCheckedToday = true;
        dayLastChecked = Main.GameTime.Day;
        Main.MessageBox.ShowForTime("Nothing is available today. Check back again tomorrow.", 5.0f, gameObject);
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
        Trigger.Reset();
    }

}
