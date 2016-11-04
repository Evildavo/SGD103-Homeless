using UnityEngine;
using System.Collections;

public class AddictionSupportEvent : EventAtLocation {
    private float hoursSpent;
    
    public float MoraleRewardPerHour;
    public float AddictionDecreasedPerHour;

    protected override void OnPlayerAttends()
    {
        hoursSpent = 0.0f;
    }

    protected override void OnPlayerLeaves()
    {
        if (hoursSpent > 0.7f && Main.PlayerState.Addiction > 0.2f)
        {
            Main.MessageBox.ShowForTime("You feel better talking about the issue", null, gameObject);
        }
    }
    
	new void Update () {
        base.Update();
        if (IsCurrentlyAttending)
        {
            hoursSpent += Main.GameTime.GameTimeDelta;

            // Give reward.
            Main.PlayerState.ChangeMorale(MoraleRewardPerHour);
            Main.PlayerState.Addiction -= AddictionDecreasedPerHour;
        }
	}
}
