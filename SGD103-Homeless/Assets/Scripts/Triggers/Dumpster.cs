using UnityEngine;
using System.Collections;

public class Dumpster : MonoBehaviour {
    private float timeAtLastCheck;

    public Trigger Trigger;
    public GameTime GameTime;
    public PlayerState PlayerState;
    public ConfirmationBox ConfirmationBox;

    public float ChanceOfFindingFoodPerSecond = 0.1f;
    public float HungerSatietyBenefit;
    public float HealthDetriment;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    public void OnTrigger()
    {
        timeAtLastCheck = Time.time;
        GameTime.TimeScale = GameTime.AcceleratedTimeScale;
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
                        PlayerState.HungerThirstSatiety += HungerSatietyBenefit;
                        PlayerState.HealthTiredness -= HealthDetriment;
                    }
                };
                ConfirmationBox.Open(onChoiceMade, "You found food. Eat it?", "Yes", "No");
                GameTime.TimeScale = GameTime.NormalTimeScale;
                Trigger.Reset();
            }
            timeAtLastCheck = Time.time;
        }
    }
    
    public void OnPlayerExit()
    {
        GameTime.TimeScale = GameTime.NormalTimeScale;
        ConfirmationBox.Close();
        Trigger.Reset();
    }
}
