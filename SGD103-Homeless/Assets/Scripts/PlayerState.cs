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

    public Color NormalTextColour = Color.black;
    public Color WarningTextColour = Color.red;
    public Color HighlightTextColour = Color.yellow;
    public bool BoldTextDuringWarning = true;
    public float HungerWarningThreshold = 0.2f;
    public float HealthWarningThreshold = 0.2f;
    public float MoraleWarningThreshold = 0.2f;

    public float Money = 0;

    [Range(0.0f, 1.0f)]
    public float HungerThirstSatiety = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Health = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Morale = 1.0f;
    [Range(0.0f, 1.0f)]
    [ReadOnly]
    public float MoraleTarget = 1.0f;
    public bool HighlightHungerThirst = false;
    public bool HighlightHealth = false;
    public bool HighlightMorale = false;
    public float HungerIncreasePerGameHour = 0.1f;
    public float HealthDecreasePerGameHour = 0.1f;
    public float MoraleMaxChangePerGameHour = 0.0f;
    public float PercentHungerAffectsMorale = 0.5f;
    public float PercentHealthAffectsMorale = 0.5f;
    public float HungerIncreaseDuringSleep = 0.01f;
    public float HealthIncreaseDuringSleep = 0.05f;
    public float SleepQualityHealthIncreaseFactor = 1.0f;

    void Update () {

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
        else if (Health <= HealthWarningThreshold)
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
            HealthText.text = "Health: " + (Health * 100).ToString("f0") + "%";
        }
        if (MoraleText)
        {
            MoraleText.text = "Morale: " + (Morale * 100).ToString("f0") + "%";
        }

        // Stats change over time.
        var gameTimeDelta = 1.0f / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        if (!SleepManager.IsAsleep)
        {
            HungerThirstSatiety -= HungerIncreasePerGameHour * gameTimeDelta;
            Health -= HealthDecreasePerGameHour * gameTimeDelta;
        }
        else
        {
            HungerThirstSatiety -= HungerIncreaseDuringSleep * gameTimeDelta;

            // Gain health based on sleep quality.
            Health += HealthIncreaseDuringSleep * 
                      SleepManager.SleepQuality * 
                      SleepQualityHealthIncreaseFactor *
                      gameTimeDelta;
        }

        // Determine morale target based on a weighted average between hunger and health.
        MoraleTarget = 
            (HungerThirstSatiety * PercentHungerAffectsMorale + Health * (1.0f - PercentHungerAffectsMorale) +
             Health * PercentHealthAffectsMorale + HungerThirstSatiety * (1.0f - PercentHealthAffectsMorale)) / 2.0f;

        // Move morale towards morale target.
        if (Morale <= MoraleTarget - MoraleMaxChangePerGameHour * gameTimeDelta)
        {
            Morale += MoraleMaxChangePerGameHour * gameTimeDelta;
        }
        else if (Morale >= MoraleTarget + MoraleMaxChangePerGameHour * gameTimeDelta)
        {
            Morale -= MoraleMaxChangePerGameHour * gameTimeDelta;
        }
        else
        {
            Morale = MoraleTarget;
        }

        // Limit stats to range 0-1.
        if (Money < 0)
        {
            Money = 0;
        }
        HungerThirstSatiety = Mathf.Clamp(HungerThirstSatiety, 0.0f, 1.0f);
        Health = Mathf.Clamp(Health, 0.0f, 1.0f);
        Morale = Mathf.Clamp(Morale, 0.0f, 1.0f);
    }
}
