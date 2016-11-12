using UnityEngine;
using System.Collections;

public class Homeless04 : Character
{
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
