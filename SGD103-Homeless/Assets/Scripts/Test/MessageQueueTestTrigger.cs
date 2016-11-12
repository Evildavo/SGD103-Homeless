using UnityEngine;
using System.Collections.Generic;

public class MessageQueueTestTrigger : MonoBehaviour
{
    private bool showingSustainedMessage = false;

    public Main Main;
    public Trigger Trigger;

    void reset()
    {
        Main.Menu.Hide();
        if (Trigger)
        {
            Trigger.Reset();
        }
        if (showingSustainedMessage)
        {
            Main.MessageBox.ShowNext();
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
        options.Add(new Menu.Option(onExit, "Exit", 0, true, null, true));

        Main.Menu.Show(options);
    }

    void onRegularMessageSelected()
    {
        Main.MessageBox.ShowForTime("Test alert message", null, gameObject);
        showingSustainedMessage = false;
        showMainMenu();
    }

    void onWarningMessageSelected()
    {
        Main.MessageBox.ShowForTime("Test warning message", null, gameObject, true);
        showingSustainedMessage = false;
        showMainMenu();
    }

    void onQueuedMessage1Selected()
    {
        Main.MessageBox.ShowQueued("Test queued message 1", null, gameObject);
        showMainMenu();
    }

    void onQueuedMessage2Selected()
    {
        Main.MessageBox.ShowQueued("Test queued message 2", null, gameObject);
        showMainMenu();
    }

    void onQueuedMessage3Selected()
    {
        Main.MessageBox.ShowQueued("Test queued message 3", null, gameObject, true);
        showMainMenu();
    }

    void onNotificationMessageSelected()
    {
        Main.MessageBox.ShowForTime("Test notification message", null, gameObject);
        showingSustainedMessage = false;
        showMainMenu();
    }

    void onSustainedMessageSelected()
    {
        Main.MessageBox.Show("Test sustained message", gameObject);
        showingSustainedMessage = true;
        showMainMenu();
    }

    void onStopSustainedMessage()
    {
        showingSustainedMessage = false;
        Main.MessageBox.ShowNext();
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