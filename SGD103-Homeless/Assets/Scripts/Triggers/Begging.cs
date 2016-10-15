using UnityEngine;
using System.Collections.Generic;

public class Begging : MonoBehaviour
{
    public Trigger Trigger;
    public WriteYourSign WriteYourSign;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    void reset()
    {
        WriteYourSign.Hide();
        Trigger.Reset();
    }
    
    public void OnTrigger()
    {
        WriteYourSign.Show();
        Trigger.GameTime.IsTimeAccelerated = false;
    }

    public void OnPlayerExit()
    {
        reset();
    }

    public void OnTriggerUpdate()
    {
        // Reset the trigger if the user closes the sign editor.
        if (!WriteYourSign.IsShown())
        {
            reset();
        }

        // Leave trigger on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }

}
