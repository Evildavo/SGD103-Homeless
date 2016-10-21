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
        if (showingSustainedMessage)
        {
            MessageBox.Hide();
        }
    }

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTrigger);
    }

    void showMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onRegularMessageSelected, "Regular message"));
        options.Add(new Menu.Option(onWarningMessageSelected, "Warning message"));
        options.Add(new Menu.Option(onQueuedMessageSelected, "Queued message"));
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

    void onRegularMessageSelected()
    {
        MessageBox.ShowForTime("Test alert message", 2.0f, gameObject);
    }

    void onWarningMessageSelected()
    {
        MessageBox.ShowForTime("Test warning message", 2.0f, gameObject, true);
    }

    void onQueuedMessageSelected()
    {
        //MessageBox.ShowQueued("Test queued message", 2.0f, gameObject);
    }

    void onNotificationMessageSelected()
    {
        MessageBox.ShowForTime("Test notification message", 2.0f, gameObject);
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
        showMainMenu();
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