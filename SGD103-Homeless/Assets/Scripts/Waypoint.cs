using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    public string GroupName;
    public Waypoint Next;
    public Waypoint Previous;
    public Waypoint Exit;
    public bool IsExitPoint;

    void Start ()
    { 
	}

    void Update()
    {
    }
}
