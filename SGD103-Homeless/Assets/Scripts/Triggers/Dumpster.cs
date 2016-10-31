using UnityEngine;
using System.Collections;

public class Dumpster : MonoBehaviour {
    private float timeAtLastCheck;

    public Main Main;
    public Trigger Trigger;

    public float ChanceOfFindingFoodPerSecond = 0.1f;
    public float HungerSatietyBenefit;
    public float HealthDetriment;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    public void OnTrigger()
    {
        timeAtLastCheck = Time.time;
        Main.GameTime.TimeScale = Main.GameTime.AcceleratedTimeScale;
    }

    public void OnTriggerUpdate()
    {
        // Every second, randomly decide if we've found food.
        if (Time.time - timeAtLastCheck > 1.0f)
        {
            float value = Random.Range(0.0f, 1.0f);
            if (value <= ChanceOfFindingFoodPerSecond)
            {
                ConfirmationBox.OnChoiceMade onChoiceMade = (bool yes) =>
                {
                    if (yes)
                    {
                        Main.PlayerState.ChangeNutrition(HungerSatietyBenefit);
                        Main.PlayerState.HealthTiredness -= HealthDetriment;
                    }
                };
                Main.ConfirmationBox.Open(onChoiceMade, "You found food. Eat it?", "Yes", "No");
                Main.GameTime.TimeScale = Main.GameTime.NormalTimeScale;
                Trigger.Reset(false);
                // TODO: Reset later.
            }
            timeAtLastCheck = Time.time;
        }
    }

    public void Reset()
    {
        Main.GameTime.TimeScale = Main.GameTime.NormalTimeScale;
        Main.ConfirmationBox.Close();
        Trigger.Reset();
    }
    
}
