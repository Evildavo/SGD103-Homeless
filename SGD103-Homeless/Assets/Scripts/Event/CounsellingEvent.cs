using UnityEngine;
using System.Collections;

public class CounsellingEvent : EventAtLocation {
    private float hoursSpent;
    
    public float MoraleRewardPerHour;

    protected override void OnPlayerAttends()
    {
        hoursSpent = 0.0f;
    }

    protected override void OnPlayerLeaves()
    {
        if (hoursSpent > 0.7f)
        {
            Main.MessageBox.ShowForTime("You feel better for having a chat", 2.0f, gameObject);
        }
    }
	
	new void Update () {
        base.Update();
        if (IsCurrentlyAttending)
        {
            hoursSpent += Main.GameTime.GameTimeDelta;

            // Give reward.
            Main.PlayerState.ChangeMorale(MoraleRewardPerHour);
        }
	}
}
