using UnityEngine;
using System.Collections;

public class CharacterTrigger : Trigger
{
    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }

    new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    new void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
