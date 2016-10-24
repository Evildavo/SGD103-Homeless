using UnityEngine;
using System.Collections;

public class SoupKitchenEvent : EventAtLocation {

    public float HungerSatietyRewardPerHour;
    public float MoraleRewardPerHour;

    protected override void OnEventFinished()
    {
        Invoke("removeHighlighting", 2.0f);
    }

    void removeHighlighting()
    {
        Main.PlayerState.HighlightHungerThirst = false;
        Main.PlayerState.HighlightMorale = false;
    }

    new void Start () {
        base.Start();
	}
	
	new void Update () {
        base.Update();

        // Give reward.
        if (IsCurrentlyAttending)
        {
            Main.PlayerState.HungerThirstSatiety += HungerSatietyRewardPerHour * Main.GameTime.GameTimeDelta;
            Main.PlayerState.GainMorale(MoraleRewardPerHour);
            Main.PlayerState.HighlightHungerThirst = true;
            Main.PlayerState.HighlightMorale = true;
        }
	}
}
