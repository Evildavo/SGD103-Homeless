﻿using UnityEngine;
using System.Collections;

public class Aman : Character
{
    public AudioClip HelloAudio;

    public float TimeCostPerConversation;
    [Header("Not given when the character says they're busy")]
    public float MoraleRewardPerConversation;

    void Start()
    {
        var trigger = GetComponent<Trigger>();
        trigger.RegisterOnTriggerListener(OnTrigger);
        trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        trigger.RegisterOnCloseRequested(Reset);
    }

    new void Update()
    {
        base.Update();
    }

    public void OnTrigger()
    {
        Speak("Keep out of trouble is my advice. How are you today?", HelloAudio);
        Main.PlayerCharacter.ShowStandardDialogueMenu(
            "Not good",
            "I'm ok",
            "What do you care?",
            onResponseChosen);
    }

    void onResponseChosen(PlayerCharacter.ResponseType response)
    {
        // Spend time having the conversation.
        Main.GameTime.SpendTime(TimeCostPerConversation);

        // Morale reward for conversation.
        Main.PlayerState.ChangeMorale(MoraleRewardPerConversation);

        if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
        {
            Main.PlayerCharacter.Speak(
                "Not good. I need your help. I've lost custody of my son because I couldn't afford to take care of him.",
                null, () =>
                {
                    float duration = Speak("I'm sorry to hear that.");
                    AddCaptionChangeCue(duration, "Take care", () =>
                    {
                        Reset();
                    });
                });
            Main.Menu.Hide();
        }
        else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
        {
            Speak("Stay out of trouble.");
            Reset();
        }
        else if (response == PlayerCharacter.ResponseType.SELFISH)
        {
            Speak("Hey! I was just trying to make conversation.");
            Reset();
        }
        else
        {
            Reset();
        }
    }

    public void OnPlayerExit()
    {
        if (IsSpeaking)
        {
            Speak("Hey, where are you going? Stay out of trouble.");
        }
    }

    public void Reset()
    {
        Main.Menu.Hide();
        GetComponent<Trigger>().Reset();
    }
}
