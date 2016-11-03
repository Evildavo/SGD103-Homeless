using UnityEngine;
using System.Collections.Generic;

public class Begging : MonoBehaviour
{
    float totalMoneyFound;
    bool hasChecked;
    float hourAtLastCheck;

    public Main Main;
    public Trigger Trigger;
    public WriteYourSign WriteYourSign;

    public float MoraleLostPerHourBegging;
    public float CheckIntervalHours;
    public float ChanceMoneyGainedAtCheck;
    public float MinAmountGained;
    public float MaxAmountGained;
    
    [Space(10.0f)]
    public bool IsBegging;

    public void StartBegging()
    {
        IsBegging = true;
        totalMoneyFound = 0.0f;
        Main.GameTime.AccelerateTime();
        Main.MessageBox.Show("Begging... ", gameObject);
        openBeggingMenu();
    }
    
    // Gets the menu for while we're begging.
    void openBeggingMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(reset, "Stop begging"));
        Main.Menu.Show(options);
    }

    void stopBegging()
    {
        Main.PlayerState.CurrentBeggingSpot = null;
        if (IsBegging)
        {
            Main.MessageBox.Hide();
            IsBegging = false;
        }

        // Gain the money and report to the player.
        Main.PlayerState.Money += totalMoneyFound;
        if (totalMoneyFound > 0.0f)
        {
            Main.MessageBox.ShowForTime("You earned $" + totalMoneyFound.ToString("f2"), 4.0f, gameObject);
        }
        else
        {
            Main.MessageBox.ShowForTime("You didn't get any money", 4.0f, gameObject);
        }
        totalMoneyFound = 0.0f;
    }

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(reset);
    }

    void reset()
    {
        stopBegging();
        WriteYourSign.Hide();
        Main.Menu.Hide();
        Trigger.Reset();
        Main.GameTime.ResetToNormalTime();
    }
    
    public void OnTrigger()
    {
        WriteYourSign.Show();
        Main.PlayerState.CurrentBeggingSpot = this;
    }

    public void OnTriggerUpdate()
    {
        if (IsBegging)
        {
            // Decrease morale while begging.
            Main.PlayerState.ChangeMorale(-MoraleLostPerHourBegging * Main.GameTime.GameTimeDelta);

            // Check at regular intervals if we got any money.
            if (!hasChecked || 
                GameTime.TimeOfDayHoursDelta(hourAtLastCheck, Main.GameTime.TimeOfDayHours).forward >= CheckIntervalHours)
            {
                hasChecked = true;
                hourAtLastCheck = Main.GameTime.TimeOfDayHours;

                // Check if we got any money.
                if (Random.Range(0.0f, 1.0f) < ChanceMoneyGainedAtCheck)
                {
                    float moneyFound = Random.Range(MinAmountGained, MaxAmountGained);
                    totalMoneyFound += moneyFound;

                    // Play audio here.
                    // TODO.
                }
            }
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
