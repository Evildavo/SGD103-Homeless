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
        Trigger.RegisterOnCloseRequested(Reset);
    }

    void Reset()
    {
        WriteYourSign.Hide();
        Trigger.Reset();
    }
    
    public void OnTrigger()
    {
        WriteYourSign.Show();
    }

    public void OnTriggerUpdate()
    {
        // Reset the trigger if the user closes the sign editor.
        if (!WriteYourSign.IsShown())
        {
            Reset();
        }
    }

}
