using UnityEngine;
using System.Collections;

public class CharacterTriggerTest : Trigger {

    public AudioSource InteractSound;
    public PlayerState PlayerState;

    public override void OnTrigger()
    {
        if (PlayerState.Money == 0.0f)
        {
            PlayerState.Money += 50.0f;
        } 
        if (InteractSound)
        {
            InteractSound.Play();
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (!IsActive && !InteractSound.isPlaying)
        {
            IsActive = true;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }
}
