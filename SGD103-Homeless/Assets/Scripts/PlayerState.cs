using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerState : MonoBehaviour {

    public GameTime GameTime;
    public PlayerSleepManager SleepManager;
    public Text MoneyText;
    public Text HungerThirstText;
    public Text HealthText;
    public Text MoraleText;

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
    public float MinSleepHealthGainPerHour = 0.0f;
    public float MaxSleepHealthGainPerHour = 0.0f;
    
    [Space(20.0f)]
    public bool HighlightHungerThirst = false;
    public bool HighlightHealth = false;
    public bool HighlightMorale = false;
        
    void Update () {
        float gameTimeDelta = 1.0f / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        bool asleep = SleepManager.IsAsleep;

        if (!asleep)
        {
            // Apply base changes.
            HungerThirstSatiety -= HungerGainPerHour * gameTimeDelta;
            HealthTiredness -= HealthLossPerHour * gameTimeDelta;
            Morale -= MoraleLossPerHour * gameTimeDelta;

            // Hunger goes down slower when satiated.
            if (HungerThirstSatiety > HungerSatiatedAtLevel)
            {
                HungerThirstSatiety += (HungerThirstSatiety - HungerSatiatedAtLevel) * MaxSatietyHungerRewardPerHour * gameTimeDelta;
            }

            // Hunger affects health.
            if (HungerThirstSatiety < HungerSatiatedAtLevel)
            {
                HealthTiredness -= (HungerSatiatedAtLevel - HungerThirstSatiety) * MaxHungerHealthPenaltyPerHour * gameTimeDelta;
            }
            else if (HungerThirstSatiety > HungerSatiatedAtLevel)
            {
                HealthTiredness += (HungerThirstSatiety - HungerSatiatedAtLevel) * MaxSatietyHealthRewardPerHour * gameTimeDelta;
            }

            // Health affects morale.
            if (HealthTiredness < HealthGoodAtLevel)
            {
                Morale -= (HealthGoodAtLevel - HealthTiredness) * MaxPoorHealthMoralePenaltyPerHour * gameTimeDelta;
            }
            else if (HealthTiredness > HealthGoodAtLevel)
            {
                Morale += (HealthTiredness - HealthGoodAtLevel) * MaxGoodHealthMoraleRewardPerHour * gameTimeDelta;
            }
        }
        else
        {
            // Hunger and morale drop at a different rate while asleep.
            HungerThirstSatiety -= HungerGainPerHour * SleepHungerGainFactor * gameTimeDelta;
            Morale -= MoraleLossPerHour * SleepMoraleLossFactor * gameTimeDelta;

            // Gain health based on sleep quality.
            float minHealthGain = MinSleepHealthGainPerHour;
            float maxHealthGain = MaxSleepHealthGainPerHour;
            HealthTiredness += (minHealthGain + SleepManager.SleepQuality * (maxHealthGain - minHealthGain)) * gameTimeDelta;
        }

        // Limit stats to range 0-1.
        if (Money < 0)
        {
            Money = 0;
        }
        HungerThirstSatiety = Mathf.Clamp(HungerThirstSatiety, 0.0f, 1.0f);
        HealthTiredness = Mathf.Clamp(HealthTiredness, 0.0f, 1.0f);
        Morale = Mathf.Clamp(Morale, 0.0f, 1.0f);

        // Highlight stats.
        HungerThirstText.fontStyle = FontStyle.Normal;
        HealthText.fontStyle = FontStyle.Normal;
        MoraleText.fontStyle = FontStyle.Normal;
        if (HighlightHungerThirst)
        {
            HungerThirstText.color = HighlightTextColour;
        }
        else if (HungerThirstSatiety <= HungerWarningThreshold)
        {
            HungerThirstText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                HungerThirstText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            HungerThirstText.color = NormalTextColour;
        }
        if (HighlightHealth)
        {
            HealthText.color = HighlightTextColour;
        }
        else if (HealthTiredness <= HealthWarningThreshold)
        {
            HealthText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                HealthText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            HealthText.color = NormalTextColour;
        }
        if (HighlightMorale)
        {
            MoraleText.color = HighlightTextColour;
        }
        else if (Morale <= MoraleWarningThreshold)
        {
            MoraleText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                MoraleText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            MoraleText.color = NormalTextColour;
        }

        // Update stat texts.
        if (MoneyText)
        {
            MoneyText.text = "$" + Money.ToString("F2");
        }
        if (HungerThirstText)
        {
            HungerThirstText.text = "Hunger/Thirst: " + (HungerThirstSatiety * 100).ToString("f0") + "%";
        }
        if (HealthText)
        {
            HealthText.text = "Health/Tiredness: " + (HealthTiredness * 100).ToString("f0") + "%";
        }
        if (MoraleText)
        {
            MoraleText.text = "Morale: " + (Morale * 100).ToString("f0") + "%";
        }
    }
}
