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
    public float HungerThirst = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Health = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Morale = 1.0f;
    [Range(0.0f, 1.0f)]
    [ReadOnly]
    public float MoraleTarget = 1.0f;
    public bool HighlightHungerThirst;
    public bool HighlightHealth;
    public bool HighlightMorale;
    public float HungerIncreasePerGameHour;
    public float HealthDecreasePerGameHour;
    public float MoraleMaxChangePerGameHour = 0.0f;
    public float PercentHungerAffectsMorale = 0.5f;
    public float PercentHealthAffectsMorale = 0.5f;
    public float WhileAsleepHungerIncreasePerGameHour;
    public float WhileAsleepHealthGainedPerGameHour;

    void Update () {

        // Highlight stats.
        HungerThirstText.fontStyle = FontStyle.Normal;
        HealthText.fontStyle = FontStyle.Normal;
        MoraleText.fontStyle = FontStyle.Normal;
        if (HighlightHungerThirst)
        {
            HungerThirstText.color = HighlightTextColour;
        }
        else if (HungerThirst <= HungerWarningThreshold)
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
            HungerThirstText.text = "Hunger/Thirst: " + (HungerThirst * 100).ToString("f0") + "%";
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
        if (!SleepManager.IsAsleep)
        {
            HungerThirst -= HungerIncreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
            Health -= HealthDecreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        }
        else
        {
            HungerThirst -= WhileAsleepHungerIncreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
            Health += WhileAsleepHealthGainedPerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        }

        // Determine morale target based on a weighted average between hunger and health.
        MoraleTarget = 
            (HungerThirst * PercentHungerAffectsMorale + Health * (1.0f - PercentHungerAffectsMorale) +
             Health * PercentHealthAffectsMorale + HungerThirst * (1.0f - PercentHealthAffectsMorale)) / 2.0f;

        // Move morale towards morale target.
        if (Morale < MoraleTarget - MoraleMaxChangePerGameHour)
        {
            Morale += MoraleMaxChangePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        }
        else if (Morale > MoraleTarget + MoraleMaxChangePerGameHour)
        {
            Morale -= MoraleMaxChangePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
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
        HungerThirst = Mathf.Clamp(HungerThirst, 0.0f, 1.0f);
        Health = Mathf.Clamp(Health, 0.0f, 1.0f);
        Morale = Mathf.Clamp(Morale, 0.0f, 1.0f);
    }
}
