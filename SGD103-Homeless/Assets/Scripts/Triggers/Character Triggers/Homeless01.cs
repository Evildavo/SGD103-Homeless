using UnityEngine;
using System.Collections;

public class Homeless01 : Character
{
    public float TimeCostPerConversation;
    public float MoraleRewardPerConversation;

    void Start()
    {
        var trigger = GetComponent<Trigger>();
        trigger.RegisterOnTriggerListener(OnTrigger);
        trigger.RegisterOnCloseRequested(Reset);
    }

    new void Update()
    {
        base.Update();
    }

    public void OnTrigger()
    {
        // Spend time having the conversation.
        Main.GameTime.SpendTime(TimeCostPerConversation);

        // Morale reward for conversation.
        Main.PlayerState.ChangeMorale(MoraleRewardPerConversation);

        Main.PlayerCharacter.Speak("Hi, how are you?", null, () =>
        {
            Speak("Good thanks");
            Reset();
        });
    }
    
    public void Reset()
    {
        GetComponent<Trigger>().Reset();
    }
}
