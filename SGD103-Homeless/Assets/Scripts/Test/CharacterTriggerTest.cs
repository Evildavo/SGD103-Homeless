﻿using UnityEngine;
using System.Collections;

public class CharacterTriggerTest : MonoBehaviour {
    
    public Trigger Trigger;
    public AudioSource InteractSound;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
    }

    public void OnTrigger()
    {
        if (InteractSound)
        {
            InteractSound.Play();
        }
    }

    public void OnTriggerUpdate()
    {
        if (!InteractSound.isPlaying)
        {
            Trigger.ResetWithCooloff();
        }
    }
}
