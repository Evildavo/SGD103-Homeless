using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Zone : MonoBehaviour
{
    public Main Main;

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
        if (Main.PlayerCharacter && other.gameObject == Main.PlayerCharacter.gameObject)
        {
            PlayerIsInside = true;
        }
    }

    // Override and call base.
    public virtual void OnTriggerExit(Collider other)
    {
        if (Main.PlayerCharacter && other.gameObject == Main.PlayerCharacter.gameObject)
        {
            PlayerIsInside = false;
        }
    }

}
