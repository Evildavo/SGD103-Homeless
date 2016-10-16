using UnityEngine;
using System.Collections;

public class PlayerSleepManager : MonoBehaviour {

    public enum SleepQualityEnum
    {
        GOOD,
        OK,
        POOR
    }

    public Transform ZoneContainer;
    public ScreenFader ScreenFader;

    [ReadOnly]
    public bool InPublic = false;
    [ReadOnly]
    public SleepQualityEnum SleepQualityHere;
    public float FadeToBlackTime = 1.5f;

    // Player goes to sleep at the current location.
    public void Sleep()
    {
        ScreenFader.fadeTime = FadeToBlackTime;
        ScreenFader.fadeIn = false;
    }

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

        // Determine the quality of sleep here based on zones the player is in.
        SleepQualityHere = SleepQualityEnum.POOR;
        SleepZone[] sleepZones = ZoneContainer.GetComponentsInChildren<SleepZone>();
        foreach (SleepZone zone in sleepZones)
        {
            if (zone.PlayerIsInside)
            {
                if (zone.HighQualitySleep)
                {
                    SleepQualityHere = SleepQualityEnum.GOOD;
                    break;
                }
                else
                {
                    SleepQualityHere = SleepQualityEnum.OK;
                }
            }
        }
    }

}
