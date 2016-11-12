using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    public Character Character;
    public Waypoint Next;

    [ReadOnly]
    public bool CharacterIsInside = false;

    void Start ()
    {
        //GetComponent<Renderer>().enabled = false;
	}

    void Update()
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Character.gameObject)
        {
            CharacterIsInside = true;
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Character.gameObject)
        {
            CharacterIsInside = false;
        }
    }
}
