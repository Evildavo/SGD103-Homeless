using UnityEngine;
using System.Collections;

public class CharacterTriggerTest : MonoBehaviour {

    public Trigger Trigger;
    public AudioSource InteractSound;
    public PlayerState PlayerState;

    public void OnTrigger()
    {
        if (InteractSound)
        {
            InteractSound.Play();
        }
    }

    void Update()
    {
        if (!Trigger.IsEnabled && !InteractSound.isPlaying)
        {
            Trigger.Reset();
        }
    }
}
