using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    public string GroupName;
    public Waypoint Previous;
    public Waypoint Next;
    public Waypoint Exit;
    public bool IsExitPoint;
    public bool TeleportToPrevious;
    public bool TeleportToNext;

    void Start ()
    { 
	}

    void Update()
    {
    }
}
