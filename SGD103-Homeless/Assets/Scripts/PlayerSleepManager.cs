using UnityEngine;
using System.Collections;

public class PlayerSleepManager : MonoBehaviour
{
    public enum SleepQualityEnum
    {
        GOOD,
        OK,
        POOR
    }
    
    public MessageBox MessageBox;
    public GameTime GameTime;
    public Transform ZoneContainer;
    public ScreenFader ScreenFader;

    [ReadOnly]
    public bool InPublic = false;
    [ReadOnly]
    public SleepQualityEnum SleepQualityHere;
    public float FadeToBlackTime = 1.5f;
    public float FadeInFromBlackTime = 1.5f;
    [ReadOnly]
    public bool IsAsleep = false;
    public float SleepTimeScale = 12000.0f;
    [Range(0.0f, 24.0f)]
    public float WakeUpAtHour = 6.5f;
    [Range(0.0f, 1.0f)]
    [ReadOnly]
    public float SleepQuality = 1.0f;

    // Player goes to sleep at the current location.
    public void Sleep()
    {
        IsAsleep = true;

        // Determine the sleep quality.
        /*if (SleepQualityHere == SleepQualityEnum.POOR)
        {
            SleepQuality = SleepQualityHere;
        }*/

        // Fade to black.
        ScreenFader.fadeTime = FadeToBlackTime;
        ScreenFader.fadeIn = false;

        // Show a sleep message and accelerate time once the fade out is complete.
        Invoke("OnFadeInComplete", FadeToBlackTime);
    }

    public void WakeUp()
    {
        IsAsleep = false;
        GameTime.TimeScale = GameTime.NormalTimeScale;

        // Show wake message.
        MessageBox.ShowForTime("You awake feeling refreshed", 2.0f, gameObject);

        // Fade in from black.
        ScreenFader.fadeTime = FadeInFromBlackTime;
        ScreenFader.fadeIn = true;
    }

    void OnFadeInComplete()
    {
        MessageBox.Show("Zzzz...", gameObject);
        GameTime.TimeScale = SleepTimeScale;
    }
	
	void Update ()
    {
        // Handle waking up from sleep. 
        if (IsAsleep)
        {
            // Wake up at a given hour.
            if (Mathf.Abs(GameTime.TimeOfDayHours - WakeUpAtHour) <= Time.deltaTime / 60.0f / 60.0f * GameTime.TimeScale)
            {
                GameTime.TimeOfDayHours = WakeUpAtHour;
                WakeUp();
            }
        }
        else
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

}
