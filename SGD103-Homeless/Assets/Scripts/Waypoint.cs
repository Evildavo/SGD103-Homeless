using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    public string GroupName;
    public Waypoint Next;

    void Start ()
    {
        //GetComponent<Renderer>().enabled = false;
	}

    void Update()
    {
    }
}
