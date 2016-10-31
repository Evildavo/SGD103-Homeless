using UnityEngine;
using System.Collections;

public class EatAtDinerEvent : EventAtLocation {
    
    public float MealCost;
    public float NutritionReward;

    protected override void OnPlayerLeaves()
    {
        Main.PlayerState.ChangeNutrition(NutritionReward);

        Main.MessageBox.ShowForTime("You feel enlivened after a good feed", 2.0f, gameObject);

        MotelDiner diner = GetComponentInParent<MotelDiner>();
        if (diner && diner.Trigger.IsActivated)
        {
            GetComponentInParent<MotelDiner>().OpenMainMenu();
        }
    }
}
