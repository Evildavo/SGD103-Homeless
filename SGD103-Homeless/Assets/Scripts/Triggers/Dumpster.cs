using UnityEngine;
using System.Collections;

public class Dumpster : MonoBehaviour {
    private float timeAtLastCheck;
    
    public PlayerState PlayerState;
    public ConfirmationBox ConfirmationBox;

    public float ChanceOfFindingFoodPerSecond = 0.1f;
    
    public void OnTrigger(Trigger trigger)
    {
        timeAtLastCheck = Time.time;
    }

    public void OnTriggerUpdate(Trigger trigger)
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
                        PlayerState.HungerThirst += 0.2f;
                        PlayerState.Health -= 0.2f;
                    }
                };
                ConfirmationBox.Open(onChoiceMade, "You found food. Eat it?", "Yes", "No");
                trigger.Reset();
            }
            timeAtLastCheck = Time.time;
        }
    }
    
    public void OnPlayerExit(Trigger trigger)
    {
        ConfirmationBox.Close();
        trigger.Reset();
    }
}
