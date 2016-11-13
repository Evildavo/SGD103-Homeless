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
    public AudioClip[] CoinClips;

    public float MoraleLostPerHourBegging;
    public float MaxChanceMoneyGainedPerPedestrianVisit;
    public float HealthAffectsChanceFactor = 1.0f;
    public float MoraleAffectsChanceFactor = 1.0f;
    public float CleanlinessAffectsChanceFactor = 1.0f;
    public float[] PossibleMoniesGained;
    [Header("There is legacy code here from before we had pedestrians.")]
    public float CheckIntervalHours;
    [UnityEngine.Serialization.FormerlySerializedAs("ChanceMoneyGainedAtCheck")]
    public float BestChanceMoneyGainedAtCheck;
    public float DisplayMoneyGainedMessageForSeconds;
    public float[] PeakHours;
    public float TimeFromPeakHourBeforeChanceIsZero = 1.0f;
    public bool BeggingAcceleratesTime = true;
    public bool OverrideAcceleratedTimeSpeed = false;
    public float AcceleratedTimeSpeed = 1.0f;
    public bool ReportMoneyGainedDuringActivity = true;

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
            if (OverrideAcceleratedTimeSpeed)
            {
                Main.GameTime.AccelerateTime(AcceleratedTimeSpeed);
            }
            else
            {
                Main.GameTime.AccelerateTime();
            }
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
            Main.MessageBox.ShowForTime("You earned $" + totalMoneyEarned.ToString("f2"), null, gameObject);
        }
        else
        {
            Main.MessageBox.ShowForTime("You didn't get any money", null, gameObject);
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
        if (IsBegging)
        {
            stopBegging();
        }
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

            // Ignore the first check.
            if (!hasChecked)
            {
                hasChecked = true;
                hourAtLastCheck = Main.GameTime.TimeOfDayHours;
            }
            else
            {
                // Check at regular intervals if we got any money.
                if (GameTime.TimeOfDayHoursDelta(hourAtLastCheck, Main.GameTime.TimeOfDayHours).forward >= CheckIntervalHours)
                {
                    hasChecked = true;
                    hourAtLastCheck = Main.GameTime.TimeOfDayHours;

                    // Determine the chance based on peak hours.
                    chanceMoneyGainedAtCheck = determineChance();

                    checkGotMoney(chanceMoneyGainedAtCheck);
                }

                // After money is gained change back to the regular searching message.
                if (Time.time - timeAtMoneyLastGained > DisplayMoneyGainedMessageForSeconds)
                {
                    showBeggingMessage();
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

    // Checks for money from a pedestrian.
    public void CheckGotMoneyPedestrian()
    {
        if (IsBegging)
        {
            // Determine chance of getting money based on health/morale/cleanliness.
            float chance = MaxChanceMoneyGainedPerPedestrianVisit * (
                Main.PlayerState.HealthTiredness * HealthAffectsChanceFactor +
                Main.PlayerState.Morale * MoraleAffectsChanceFactor +
                Main.PlayerState.CurrentClothingCleanliness * CleanlinessAffectsChanceFactor) / 3f;

            checkGotMoney(chance);
        }
    }

    void checkGotMoney(float chance)
    {
        // Check if we got any money.
        if (Random.Range(0.0f, 1.0f) < chance)
        {
            float moneyEarned = PossibleMoniesGained[Random.Range(0, PossibleMoniesGained.Length)];
            totalMoneyEarned += moneyEarned;

            // Play random coin sound.
            var audio = GetComponent<AudioSource>();
            audio.clip = CoinClips[Random.Range(0, CoinClips.Length)];
            audio.Play();

            // Display message that money was gained.
            if (ReportMoneyGainedDuringActivity)
            {
                Main.MessageBox.SetMessage("$" + moneyEarned.ToString("f2") + " gained");
            }
            else
            {
                Main.MessageBox.SetMessage("Money gained");
            }
            timeAtMoneyLastGained = Time.time;
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
