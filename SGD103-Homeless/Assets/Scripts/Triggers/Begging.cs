using UnityEngine;
using System.Collections.Generic;

public class Begging : MonoBehaviour
{
    float totalMoneyEarned;
    bool hasChecked;
    float hourAtLastCheck;
    float timeAtMoneyLastGained;

    public Main Main;
    public Trigger Trigger;
    public WriteYourSign WriteYourSign;

    public float MoraleLostPerHourBegging;
    public float CheckIntervalHours;
    public float ChanceMoneyGainedAtCheck;
    public float MinAmountGained;
    public float MaxAmountGained;
    public float DisplayMoneyGainedMessageForSeconds;

    [Space(10.0f)]
    public bool IsBegging;

    public void StartBegging()
    {
        IsBegging = true;
        totalMoneyEarned = 0.0f;
        Main.GameTime.AccelerateTime();
        showBeggingMessage();
        openBeggingMenu();
    }

    void showBeggingMessage()
    {
        Main.MessageBox.Show("Begging... ", gameObject);
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
        Main.PlayerState.Money += totalMoneyEarned;
        if (totalMoneyEarned > 0.0f)
        {
            Main.MessageBox.ShowForTime("You earned $" + totalMoneyEarned.ToString("f2"), 4.0f, gameObject);
        }
        else
        {
            Main.MessageBox.ShowForTime("You didn't get any money", 4.0f, gameObject);
        }
        totalMoneyEarned = 0.0f;
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
        Main.MessageBox.Hide();
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
                    float moneyEarned = Random.Range(MinAmountGained, MaxAmountGained);
                    totalMoneyEarned += moneyEarned;

                    // Play audio here.
                    // TODO.

                    // Display message that money was gained.
                    Main.MessageBox.SetMessage("Money gained");
                    timeAtMoneyLastGained = Main.GameTime.TimeOfDayHours;
                }
            }

            // After money is gained change back to the regular searching message.
            if (GameTime.TimeOfDayHoursDelta(timeAtMoneyLastGained, Main.GameTime.TimeOfDayHours).forward > 
                DisplayMoneyGainedMessageForSeconds)
            {
                showBeggingMessage();
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
