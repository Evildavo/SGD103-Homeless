using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerState : MonoBehaviour {

    public GameTime GameTime;
    public Text MoneyText;
    public Text HungerThirstText;
    public Text HealthText;
    public Text MoraleText;

    public Color NormalTextColour = Color.black;
    public Color HighlightTextColour = Color.black;

    public float Money = 0;

    [Range(0.0f, 1.0f)]
    public float HungerThirst = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Health = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Morale = 1.0f;
    public bool HighlightHungerThirst;
    public bool HighlightHealth;
    public bool HighlightMorale;
    public float HungerIncreasePerGameHour;
    public float HealthDecreasePerGameHour;
    public float MoraleDecreasePerGameHour = 0.0f;
    public float PercentHungerAffectsMorale = 0.5f;
    public float PercentHealthAffectsMorale = 0.5f;
    public float OkHungerLevel = 0.85f;
    public float OkHealthLevel = 0.85f;

    void Update () {

        // Highlight stats.
        if (HighlightHungerThirst)
        {
            HungerThirstText.color = HighlightTextColour;
        }
        else
        {
            HungerThirstText.color = NormalTextColour;
        }
        if (HighlightHealth)
        {
            HealthText.color = HighlightTextColour;
        }
        else
        {
            HealthText.color = NormalTextColour;
        }
        if (HighlightMorale)
        {
            MoraleText.color = HighlightTextColour;
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
        
        // Penalise morale based on hunger and health.
        var moralePenalty = (OkHungerLevel - HungerThirst) * PercentHungerAffectsMorale +
                            (OkHealthLevel - Health) * PercentHealthAffectsMorale;

        // Decrease stats over time.
        HungerThirst -= HungerIncreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        Health -= HealthDecreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        Morale -= (MoraleDecreasePerGameHour + moralePenalty) / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;

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
