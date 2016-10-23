using UnityEngine;
using System.Collections.Generic;

public class ModelDiner : MonoBehaviour
{
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public Menu Menu;
    public MessageBox MessageBox;
    public JobLocation JobLocation;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
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
        Menu.Show(options);
    }

    public void ApplyForJob()
    {
        JobLocation.ApplyForJob();
        OpenMainMenu();
    }

    void Reset()
    {
        Menu.Hide();
        MessageBox.ShowNext();
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
            MessageBox.ShowForTime("Work is about to start.", 2.0f, gameObject);
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
