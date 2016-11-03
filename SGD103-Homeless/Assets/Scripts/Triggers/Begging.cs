using UnityEngine;
using System.Collections.Generic;

public class Begging : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;
    public WriteYourSign WriteYourSign;

    public bool IsBegging;

    public float MoraleLostPerHourBegging;

    public void StartBegging()
    {
        IsBegging = true;
        Main.GameTime.AccelerateTime();
        Main.MessageBox.Show("Begging... ", gameObject);
        openBeggingMenu();
    }
    
    // Gets the menu for while we're begging.
    void openBeggingMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(StopBegging, "Stop begging"));
        Main.Menu.Show(options);
    }

    public void StopBegging()
    {
        Main.MessageBox.Hide();
        reset();
    }

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(reset);
    }

    void reset()
    {
        WriteYourSign.Hide();
        Main.Menu.Hide();
        Trigger.Reset();
        Main.PlayerState.CurrentBeggingSpot = null;
        Main.GameTime.ResetToNormalTime();

        if (IsBegging)
        {
            Main.MessageBox.Hide();
            IsBegging = false;
        }
    }
    
    public void OnTrigger()
    {
        WriteYourSign.Show();
        Main.PlayerState.CurrentBeggingSpot = this;
    }

    public void OnTriggerUpdate()
    {
        // Decrease morale while begging.
        if (IsBegging)
        {
            Main.PlayerState.ChangeMorale(-MoraleLostPerHourBegging * Main.GameTime.GameTimeDelta);
        }
        else
        {
            // Reset the trigger if the user closes the sign editor.
            if (!WriteYourSign.IsShown())
            {
                reset();
            }
        }
    }

}
