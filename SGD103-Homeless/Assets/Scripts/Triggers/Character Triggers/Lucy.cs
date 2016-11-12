using UnityEngine;
using System.Collections;

public class Lucy : Character
{
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
        Speak("Hello");
        Main.PlayerCharacter.ShowStandardDialogueMenu(
            "Submissive",
            "Prideful",
            "Selfish",
            onResponseChosen);
    }

    void onResponseChosen(PlayerCharacter.ResponseType response)
    {
        if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
        {
            Speak("Ok");
            Reset();
        }
        else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
        {
            Speak("Ok");
            Reset();
        }
        else if (response == PlayerCharacter.ResponseType.SELFISH)
        {
            Speak("Ok");
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
            Speak("Hey, where are you going?");
        }
    }

    public void Reset()
    {
        Main.Menu.Hide();
        GetComponent<Trigger>().Reset();
    }
}
