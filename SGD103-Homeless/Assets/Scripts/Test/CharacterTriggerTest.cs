using UnityEngine;
using System.Collections;

public class CharacterTriggerTest : MonoBehaviour {
    
    public AudioSource InteractSound;
    public PlayerState PlayerState;

    public void OnTrigger(Trigger trigger)
    {
        if (InteractSound)
        {
            InteractSound.Play();
        }
    }

    public void OnTriggerUpdate(Trigger trigger)
    {
        if (!InteractSound.isPlaying)
        {
            trigger.Reset();
        }
    }
}
