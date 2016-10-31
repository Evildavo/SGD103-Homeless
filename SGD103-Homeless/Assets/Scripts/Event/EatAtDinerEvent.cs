using UnityEngine;
using System.Collections;

public class EatAtDinerEvent : EventAtLocation {
    
    public float MealCost;
    public float HungerSatietyReward;

    protected override void OnPlayerLeaves()
    {
        Main.PlayerState.ChangeNutrition(HungerSatietyReward);

        Main.MessageBox.ShowForTime("You feel enlivened after a good feed", 2.0f, gameObject);

        MotelDiner diner = GetComponentInParent<MotelDiner>();
        if (diner && diner.Trigger.IsActivated)
        {
            GetComponentInParent<MotelDiner>().OpenMainMenu();
        }
    }
}
