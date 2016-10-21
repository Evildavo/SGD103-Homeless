using UnityEngine;
using System.Collections.Generic;

public class MessageQueueTestTrigger : MonoBehaviour
{
    private bool showingSustainedMessage = false;

    public Trigger Trigger;
    public MessageBox MessageBox;
    public Menu Menu;

    void reset()
    {
        Menu.Hide();
        if (Trigger)
        {
            Trigger.Reset();
        }
    }

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTrigger);
    }

    void openMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onAlertMessageSelected, "Queued message"));
        options.Add(new Menu.Option(onAlertMessageSelected, "Alert message"));
        options.Add(new Menu.Option(onWarningMessageSelected, "Warning message"));
        options.Add(new Menu.Option(onNotificationMessageSelected, "Notification message"));
        if (showingSustainedMessage)
        {
            options.Add(new Menu.Option(onStopSustainedMessage, "Stop sustained message"));
        }
        else 
        {
            options.Add(new Menu.Option(onSustainedMessageSelected, "Sustained message"));
        }
        options.Add(new Menu.Option(onExit, "Exit"));
        
        Menu.Show(options);
    }

    void onQueuedMessageSelected()
    {
        //MessageBox.ShowQueued("Test queued message", 2.0f, gameObject, false);
        showingSustainedMessage = false;
    }

    void onAlertMessageSelected()
    {
        MessageBox.ShowForTime("Test alert message", 2.0f, gameObject, false);
        showingSustainedMessage = false;
    }

    void onWarningMessageSelected()
    {
        MessageBox.ShowForTime("Test warning message", 2.0f, gameObject, true);
        showingSustainedMessage = false;
    }

    void onNotificationMessageSelected()
    {
        MessageBox.ShowForTime("Test notification message", 2.0f, gameObject);
        showingSustainedMessage = false;
    }

    void onSustainedMessageSelected()
    {
        MessageBox.Show("Test sustained message", gameObject);
        showingSustainedMessage = true;
    }

    void onStopSustainedMessage()
    {
        MessageBox.Hide();
        showingSustainedMessage = false;
    }

    void onExit()
    {
        reset();
    }

    public void OnTrigger()
    {
        reset();
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