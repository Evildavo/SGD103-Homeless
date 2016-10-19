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
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
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
        options.Add(new Menu.Option(OnExit, "Exit"));
        Menu.Show(options);
    }

    public void ApplyForJob()
    {
        JobLocation.ApplyForJob();
        OpenMainMenu();
    }

    void reset()
    {
        Menu.Hide();
        MessageBox.Hide();
        if (Trigger)
        {
            Trigger.Reset(Trigger.IsEnabled);
        }
    }

    public void OnExit()
    {
        reset();
    }

    public void OnTrigger()
    {
        JobLocation.CheckForJob(true);
        OpenMainMenu();
    }

    public void OnPlayerExit()
    {
        reset();
    }

    public void OnTriggerUpdate()
    {
        // Show warning that job is about to start.
        if (JobLocation.CanWorkNow)
        {
            reset();
            MessageBox.ShowForTime("Work is about to start.", 2.0f, gameObject);
        }

        // Leave menu on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
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
