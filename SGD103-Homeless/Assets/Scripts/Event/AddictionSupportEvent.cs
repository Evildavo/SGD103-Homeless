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
            Main.MessageBox.ShowForTime("You feel better talking about the issue", 2.0f, gameObject);
        }
        Invoke("removeHighlighting", 2.0f);
    }

    void removeHighlighting()
    {
        Main.PlayerState.HighlightMorale = false;
    }
	
	new void Update () {
        base.Update();
        if (IsCurrentlyAttending)
        {
            hoursSpent += Main.GameTime.GameTimeDelta;

            // Give reward.
            Main.PlayerState.GainMorale(MoraleRewardPerHour);
            Main.PlayerState.Addiction -= AddictionDecreasedPerHour;
            Main.PlayerState.HighlightMorale = true;
        }
	}
}
