﻿using UnityEngine;
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
    public AudioClip[] CoinClips;

    public float MoraleLostPerHourBegging;
    public float CheckIntervalHours;
    [UnityEngine.Serialization.FormerlySerializedAs("ChanceMoneyGainedAtCheck")]
    public float BestChanceMoneyGainedAtCheck;
    public float MinAmountGained;
    public float MaxAmountGained;
    public float DisplayMoneyGainedMessageForSeconds;
    public float[] PeakHours;
    public float TimeFromPeakHourBeforeChanceIsZero = 1.0f;
    public bool BeggingAcceleratesTime = true;

    [Space(10.0f)]
    public bool IsBegging;
    [ReadOnly]
    public float chanceMoneyGainedAtCheck;

    public void StartBegging()
    {
        IsBegging = true;
        totalMoneyEarned = 0.0f;
        if (BeggingAcceleratesTime)
        {
            Main.GameTime.AccelerateTime();
        }
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

                // Determine the chance based on peak hours.
                chanceMoneyGainedAtCheck = determineChance();

                // Check if we got any money.
                if (Random.Range(0.0f, 1.0f) < chanceMoneyGainedAtCheck)
                {
                    float moneyEarned = Random.Range(MinAmountGained, MaxAmountGained);

                    // Round to the nearest 5 cents.
                    moneyEarned = Mathf.Round(moneyEarned * 20.0f) / 20.0f;

                    totalMoneyEarned += moneyEarned;

                    // Play random coin sound.
                    var audio = GetComponent<AudioSource>();
                    audio.clip = CoinClips[Random.Range(0, CoinClips.Length)];
                    audio.Play();

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

    float determineChance()
    {
        // Find the distance to the closest peak hour to the present.
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < PeakHours.Length; i++)
        {
            float delta = GameTime.TimeOfDayHoursDelta(Main.GameTime.TimeOfDayHours, PeakHours[i]).shortest;
            if (delta < nearestDistance)
            {
                nearestDistance = delta;
            }
        }
        
        // Money gained is based on the distance to the closest peak hour.
        return Mathf.Max(
            BestChanceMoneyGainedAtCheck * 
            (1.0f - nearestDistance / TimeFromPeakHourBeforeChanceIsZero), 0.0f);
    }

}
