using UnityEngine;
using System.Collections;

public class Greg : MonoBehaviour {

    public CharacterTrigger CharacterTrigger;
    
	void Start ()
    {
        CharacterTrigger.RegisterOnTriggerListener(OnTrigger);
        CharacterTrigger.RegisterOnPlayerExitListener(OnPlayerExit);
        CharacterTrigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
    }

    public void OnTrigger()
    {
        Debug.Log("Talking to Greg");
        reset();
    }

    public void OnPlayerExit()
    {
        reset();
    }

    public void OnTriggerUpdate()
    {
        // Leave trigger on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }

    void reset()
    {
        CharacterTrigger.Reset();
        if (CharacterTrigger)
        {
            CharacterTrigger.ResetWithCooloff();
        }
    }

}
