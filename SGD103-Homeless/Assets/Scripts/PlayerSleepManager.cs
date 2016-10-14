using UnityEngine;
using System.Collections;

public class PlayerSleepManager : MonoBehaviour {

    public Transform ZoneContainer;
    [ReadOnly]
    public bool InPublic = false;
    
	void Start () {
	
	}
	
	void Update ()
    {
        // Determine if we're in a public zone.
        InPublic = false;
        PublicZone[] publicZones = ZoneContainer.GetComponentsInChildren<PublicZone>();
        foreach (PublicZone zone in publicZones)
        {
            if (zone.PlayerIsInside)
            {
                InPublic = true;
                break;
            }
        }
	}
}
