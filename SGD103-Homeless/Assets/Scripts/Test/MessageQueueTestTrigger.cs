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
            MessageBox.ShowNext();
        }
    }

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
    }

    void showMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onRegularMessageSelected, "Regular message"));
        options.Add(new Menu.Option(onWarningMessageSelected, "Warning message"));
        options.Add(new Menu.Option(onQueuedMessage1Selected, "Queued message 1"));
        options.Add(new Menu.Option(onQueuedMessage2Selected, "Queued message 2"));
        options.Add(new Menu.Option(onQueuedMessage3Selected, "Queued message 3"));
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
        showingSustainedMessage = false;
        showMainMenu();
    }

    void onWarningMessageSelected()
    {
        MessageBox.ShowForTime("Test warning message", 2.0f, gameObject, true);
        showingSustainedMessage = false;
        showMainMenu();
    }

    void onQueuedMessage1Selected()
    {
        MessageBox.ShowQueued("Test queued message 1", 2.0f, gameObject);
        showMainMenu();
    }

    void onQueuedMessage2Selected()
    {
        MessageBox.ShowQueued("Test queued message 2", 4.0f, gameObject);
        showMainMenu();
    }

    void onQueuedMessage3Selected()
    {
        MessageBox.ShowQueued("Test queued message 3", 2.0f, gameObject, true);
        showMainMenu();
    }

    void onNotificationMessageSelected()
    {
        MessageBox.ShowForTime("Test notification message", 2.0f, gameObject);
        showingSustainedMessage = false;
        showMainMenu();
    }

    void onSustainedMessageSelected()
    {
        MessageBox.Show("Test sustained message", gameObject);
        showingSustainedMessage = true;
        showMainMenu();
    }

    void onStopSustainedMessage()
    {
        showingSustainedMessage = false;
        MessageBox.ShowNext();
        showMainMenu();
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