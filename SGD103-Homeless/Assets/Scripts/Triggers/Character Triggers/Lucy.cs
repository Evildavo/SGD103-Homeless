using UnityEngine;
using System.Collections;

public class Lucy : Character
{
    enum Reputation
    {
        GOOD,
        STANDARD,
        BAD
    }

    Reputation reputation = Reputation.STANDARD;

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
        if (reputation == Reputation.STANDARD)
        {
            Speak("(cheerful) Hey there chin up, we've got a big day of stuff to do! ");
            Main.PlayerCharacter.ShowStandardDialogueMenu(
                "1. Anything in mind?",
                "2. That's alright... ",
                "3. Get any money from that job you applied for yet?",
                onResponseChosen);
        }
        else if (reputation == Reputation.GOOD)
        {

        }
        else
        {
            Speak("Leave me alone");
            Reset();
        }
    }

    void onResponseChosen(PlayerCharacter.ResponseType response)
    {
        if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
        {
            Speak("Well I was thinking of heading to the library later", null);
            AddCaptionChangeCue(3, "its always good to look for jobs and improve the resume.");
            AddCaptionChangeCue(6, "Plus reading always used to be a good way to pass the time.");
            SetFinishCallback(() =>
            {
                Main.PlayerCharacter.Speak("Okay i'll have a browse if i'm free then.", null, () =>
                {
                    Speak("Oh ok");
                    Reset();
                });
            });
        }
        else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
        {
            Main.PlayerCharacter.Speak("That's alright I've got my own stuff I need to take care of", null, () =>
            {
                Speak("Ok");
                Reset();
            });
        }
        else if (response == PlayerCharacter.ResponseType.SELFISH)
        {
            Speak("Well fuck you then!");
            reputation = Reputation.BAD;
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
