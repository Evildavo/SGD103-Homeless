using UnityEngine;
using System.Collections;

public class EatAtDinerEvent : EventAtLocation {
    
    public float MealCost;
    public float HungerSatietyReward;
        
    protected override void OnPlayerAttends()
    {
        Main.PlayerState.HighlightHungerThirst = true;
    }

    protected override void OnPlayerLeaves()
    {
        Main.PlayerState.HungerThirstSatiety += HungerSatietyReward;

        Main.MessageBox.ShowForTime("You feel enlivened after a good feed", 2.0f, gameObject);
        Invoke("removeHighlighting", 2.0f);

        GetComponentInParent<MotelDiner>().OpenMainMenu();
    }

    void removeHighlighting()
    {
        Main.PlayerState.HighlightHungerThirst = false;
    }
}
