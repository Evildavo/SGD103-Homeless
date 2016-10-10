﻿using UnityEngine;
using System.Collections;

public class Dumpster : MonoBehaviour {

    public Trigger Trigger;
    public PlayerState PlayerState;
    public ConfirmationBox ConfirmationBox;
    
    public void OnTrigger()
    {
        const float INTERVAL_SECONDS = 1.0f;
        Trigger.Start(INTERVAL_SECONDS);
    }

    public void OnTriggerUpdate()
    {
        float value = Random.Range(0, 10);
        if (value == 0)
        {
            ConfirmationBox.OnConfirmation onEatFoodConfirmed = () =>
            {
                PlayerState.HungerThirst += 0.2f;
                PlayerState.Health -= 0.2f;
            };
            ConfirmationBox.Open(onEatFoodConfirmed, "You found food. Eat it?", "Yes", "No");
            Trigger.Stop();
            Trigger.Reset();
        }
    }
    
    public void OnPlayerExit()
    {
        Trigger.Stop();
        Trigger.Reset();
    }
}
