using UnityEngine;
using System.Collections;

public class SoupKitchenEvent : EventAtLocation {
    private float hoursSpent;

    public float HungerSatietyRewardPerHour;
    public float MoraleRewardPerHour;

    protected override void OnPlayerAttends()
    {
        hoursSpent = 0.0f;
    }

    protected override void OnPlayerLeaves()
    {
        if (hoursSpent > 0.7f)
        {
            Main.MessageBox.ShowForTime("You feel enlivened after a good feed", 2.0f, gameObject);
        }
    }

	new void Update () {
        base.Update();
        if (IsCurrentlyAttending)
        {
            hoursSpent += Main.GameTime.GameTimeDelta;

            // Give reward.
            Main.PlayerState.HungerThirstSatiety += HungerSatietyRewardPerHour * Main.GameTime.GameTimeDelta;
            Main.PlayerState.ChangeMorale(MoraleRewardPerHour);
        }
	}
}
