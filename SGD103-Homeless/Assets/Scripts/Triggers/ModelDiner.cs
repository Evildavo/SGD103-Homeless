using UnityEngine;
using System.Collections.Generic;

public class ModelDiner : MonoBehaviour
{
    public Trigger Trigger;
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
            options.Add(new Menu.Option(JobLocation.ApplyForJob, "Apply for job"));
        }
        options.Add(new Menu.Option(OnExit, "Exit"));
        Menu.Show(options);
    }

    void reset()
    {
        Menu.Hide();
        MessageBox.Hide();
        if (Trigger)
        {
            Trigger.Reset();
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
        // Leave menu on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }
}
