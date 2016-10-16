using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Zone : MonoBehaviour
{
    public PlayerCharacter PlayerCharacter;

    [ReadOnly]
    public bool PlayerIsInside = false;

    // Override and call base.
    public virtual void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    // Override and call base.
    public virtual void Update() {}

    // Override and call base.
    public virtual void OnTriggerEnter(Collider other)
    {
        if (PlayerCharacter && other.gameObject == PlayerCharacter.gameObject)
        {
            PlayerIsInside = true;
        }
    }

    // Override and call base.
    public virtual void OnTriggerExit(Collider other)
    {
        if (PlayerCharacter && other.gameObject == PlayerCharacter.gameObject)
        {
            PlayerIsInside = false;
        }
    }

}
