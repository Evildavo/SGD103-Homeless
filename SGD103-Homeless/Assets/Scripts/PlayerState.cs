using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerState : MonoBehaviour {

    public GameTime GameTime;
    public Text MoneyText;
    public Text HungerThirstText;
    public Text HealthText;
    public Text MoraleText;

    public float Money = 0;

    [Range(0.0f, 1.0f)]
    public float HungerThirst = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Health = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Morale = 1.0f;
    public float HungerIncreasePerGameHour;
    public float HealthDecreasePerGameHour;
    public float MoraleDecreasePerGameHour;
	
	void Update () {

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

        // Decrease stats over time.
        HungerThirst -= HungerIncreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        if (HungerThirst > 1.0f)
        {
            HungerThirst = 1.0f;
        }
        Health -= HealthDecreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        if (Health > 1.0f)
        {
            Health = 1.0f;
        }
        Morale -= MoraleDecreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        if (Morale > 1.0f)
        {
            Morale = 1.0f;
        }
    }
}
