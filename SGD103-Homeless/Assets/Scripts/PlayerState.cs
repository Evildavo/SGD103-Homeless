using UnityEngine;

public class PlayerState : MonoBehaviour {

    public Main Main;

    [Header("Display settings:")]
    public Color NormalTextColour = Color.black;
    public Color WarningTextColour = Color.red;
    public Color HighlightTextColour = Color.yellow;
    public bool BoldTextDuringWarning = true;
    public float HungerWarningThreshold = 0.2f;
    public float HealthWarningThreshold = 0.2f;
    public float MoraleWarningThreshold = 0.2f;

    [Header("Stats:")]
    public float Money = 0;
    [Range(0.0f, 1.0f)]
    public float HungerThirstSatiety = 1.0f;
    [Range(0.0f, 1.0f)]
    public float HealthTiredness = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Morale = 1.0f;

    [Space(10.0f)]
    [Range(0.0f, 1.0f)]
    public float Inebriation = 0.0f;
    [Range(0.0f, 1.0f)]
    public float CurrentClothingCleanliness = 0.0f;

    [Header("Settings:")] 
    public float HungerGainPerHour = 0.0f;        
    public float HealthLossPerHour = 0.0f;
    public float MoraleLossPerHour = 0.0f;
    
    [Space(10.0f)]
    public float MaxHungerHealthPenaltyPerHour = 0.0f;
    [Range(0.0f, 1.0f)]
    public float HungerSatiatedAtLevel = 0.0f;
    public float MaxSatietyHealthRewardPerHour = 0.0f;

    [Header("Hunger goes down slower when satiated")]
    public float MaxSatietyHungerRewardPerHour = 0.0f;

    [Space(10.0f)]
    public float MaxPoorHealthMoralePenaltyPerHour = 0.0f;
    [Range(0.0f, 1.0f)]
    public float HealthGoodAtLevel = 0.0f;
    public float MaxGoodHealthMoraleRewardPerHour = 0.0f;

    [Space(10.0f)]
    public float SleepHungerGainFactor = 0.0f;
    public float SleepMoraleLossFactor = 0.0f;
    [Header("Not counting bonus from a sleeping bag")]
    public float MinSleepingRoughHealthGainPerHour = 0.0f;
    public float MaxSleepingRoughHealthGainPerHour = 0.0f;

    [Space(10.0f)]
    public float WorkHungerGainFactor;
    public float WorkHealthLossFactor;
    public float WorkMoraleLossFactor;
    
    [Space(10.0f)]
    public bool IsAtWork = false;

    [Space(10.0f)]
    public Trigger CurrentTrigger;

    [Space(10.0f)]
    public bool HighlightHungerThirst = false;
    public bool HighlightHealth = false;
    public bool HighlightMorale = false;

    // Returns true if the player can currently afford the given price.
    public bool CanAfford(float price)
    {
        return (Money >= price);
    }
        
    void Update () {
        float gameTimeDelta = 1.0f / 60.0f / 60.0f * Time.deltaTime * Main.GameTime.TimeScale;  
        
        if (Main.SleepManager.IsAsleep)
        {
            // Hunger and morale drop at a different rate while asleep.
            HungerThirstSatiety -= HungerGainPerHour * SleepHungerGainFactor * gameTimeDelta;
            Morale -= MoraleLossPerHour * SleepMoraleLossFactor * gameTimeDelta;

            // Gain health based on sleep quality.
            float minHealthGain = MinSleepingRoughHealthGainPerHour;
            float maxHealthGain = MaxSleepingRoughHealthGainPerHour;
            HealthTiredness += (minHealthGain + Main.SleepManager.SleepQuality * (maxHealthGain - minHealthGain)) * gameTimeDelta;
        }
        else if (IsAtWork)
        {
            // Hunger, health and morale drop at a different rate while at work.
            HungerThirstSatiety -= HungerGainPerHour * WorkHungerGainFactor * gameTimeDelta;
            HealthTiredness -= HealthLossPerHour * WorkHealthLossFactor * gameTimeDelta;
            Morale -= MoraleLossPerHour * WorkMoraleLossFactor * gameTimeDelta;
        }
        else
        {
            // Apply base changes.
            HungerThirstSatiety -= HungerGainPerHour * gameTimeDelta;
            HealthTiredness -= HealthLossPerHour * gameTimeDelta;
            Morale -= MoraleLossPerHour * gameTimeDelta;
        }

        // Hunger goes down slower when satiated.
        if (HungerThirstSatiety > HungerSatiatedAtLevel)
        {
            HungerThirstSatiety +=
                (HungerThirstSatiety - HungerSatiatedAtLevel) / (1.0f - HungerSatiatedAtLevel) *
                MaxSatietyHungerRewardPerHour * gameTimeDelta;
        }

        // Hunger affects health.
        if (HungerThirstSatiety < HungerSatiatedAtLevel)
        {
            HealthTiredness -=
                (HungerSatiatedAtLevel - HungerThirstSatiety) / HungerSatiatedAtLevel *
                MaxHungerHealthPenaltyPerHour * gameTimeDelta;
        }
        else if (HungerThirstSatiety > HungerSatiatedAtLevel)
        {
            HealthTiredness +=
                (HungerThirstSatiety - HungerSatiatedAtLevel) / (1.0f - HungerSatiatedAtLevel) *
                MaxSatietyHealthRewardPerHour * gameTimeDelta;
        }

        // Health affects morale.
        if (HealthTiredness < HealthGoodAtLevel)
        {
            Morale -=
                (HealthGoodAtLevel - HealthTiredness) / HealthGoodAtLevel *
                MaxPoorHealthMoralePenaltyPerHour * gameTimeDelta;
        }
        else if (HealthTiredness > HealthGoodAtLevel)
        {
            Morale +=
                (HealthTiredness - HealthGoodAtLevel) / (1.0f - HealthGoodAtLevel) *
                MaxGoodHealthMoraleRewardPerHour * gameTimeDelta;
        }

        // Limit stats to range 0-1.
        if (Money < 0)
        {
            Money = 0;
        }
        HungerThirstSatiety = Mathf.Clamp(HungerThirstSatiety, 0.0f, 1.0f);
        HealthTiredness = Mathf.Clamp(HealthTiredness, 0.0f, 1.0f);
        Morale = Mathf.Clamp(Morale, 0.0f, 1.0f);
        Inebriation = Mathf.Clamp(Inebriation, 0.0f, 1.0f);
        CurrentClothingCleanliness = Mathf.Clamp(CurrentClothingCleanliness, 0.0f, 1.0f);

        // Highlight stats.
        var stats = Main.StatPanel;
        stats.HungerThirstText.fontStyle = FontStyle.Normal;
        stats.HealthText.fontStyle = FontStyle.Normal;
        stats.MoraleText.fontStyle = FontStyle.Normal;
        if (HighlightHungerThirst)
        {
            stats.HungerThirstText.color = HighlightTextColour;
        }
        else if (HungerThirstSatiety <= HungerWarningThreshold)
        {
            stats.HungerThirstText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                stats.HungerThirstText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            stats.HungerThirstText.color = NormalTextColour;
        }
        if (HighlightHealth)
        {
            stats.HealthText.color = HighlightTextColour;
        }
        else if (HealthTiredness <= HealthWarningThreshold)
        {
            stats.HealthText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                stats.HealthText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            stats.HealthText.color = NormalTextColour;
        }
        if (HighlightMorale)
        {
            stats.MoraleText.color = HighlightTextColour;
        }
        else if (Morale <= MoraleWarningThreshold)
        {
            stats.MoraleText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                stats.MoraleText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            stats.MoraleText.color = NormalTextColour;
        }

        // Update stat texts.
        if (Main.MoneyPanel.MoneyText)
        {
            Main.MoneyPanel.MoneyText.text = "$" + Money.ToString("F2");
        }
        if (stats.HungerThirstText)
        {
            stats.HungerThirstText.text = "Hunger/Thirst: " + (HungerThirstSatiety * 100).ToString("f0") + "%";
        }
        if (stats.HealthText)
        {
            stats.HealthText.text = "Health/Tiredness: " + (HealthTiredness * 100).ToString("f0") + "%";
        }
        if (stats.MoraleText)
        {
            stats.MoraleText.text = "Morale: " + (Morale * 100).ToString("f0") + "%";
        }
    }
}
