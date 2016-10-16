﻿using UnityEngine;
using System.Collections;

public class PlayerSleepManager : MonoBehaviour
{
    private SleepQualityEnum sleepQualityAtSleep;
    public float hoursSlept;
    private float timeSinceLastHour;

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
    public float WakeUpHour = 6.5f;
    public float MaxSleepHours = 12.0f;
    [Range(0.0f, 1.0f)]
    [ReadOnly]
    public float SleepQuality = 1.0f;
    public float BaseGoodSleepQuality = 1.0f;
    public float BaseOkSleepQuality = 0.5f;
    public float BasePoorSleepQuality = 0.15f;
    public float ChanceOfBeingWokenInPublicPerHour = 0.3f;
    [Range(0.0f, 24.0f)]
    public float CanBeWokenInPublicFromHour = 6.5f;
    [Range(0.0f, 24.0f)]
    public float CanBeWokenInPublicToHour = 22.0f;

    // Player goes to sleep at the current location.
    public void Sleep()
    {
        if (!IsAsleep)
        {
            IsAsleep = true;
            hoursSlept = 0.0f;
            timeSinceLastHour = 0.0f;

            // Determine the quality of our sleep.
            sleepQualityAtSleep = SleepQualityHere;
            switch (SleepQualityHere)
            {
                case SleepQualityEnum.POOR:
                    SleepQuality = BasePoorSleepQuality;
                    break;
                case SleepQualityEnum.OK:
                    SleepQuality = BaseOkSleepQuality;
                    break;
                case SleepQualityEnum.GOOD:
                    SleepQuality = BaseGoodSleepQuality;
                    break;
            }

            // Fade to black.
            ScreenFader.fadeTime = FadeToBlackTime;
            ScreenFader.fadeIn = false;

            // Show a sleep message and accelerate time once the fade out is complete.
            Invoke("OnFadeInComplete", FadeToBlackTime);
        }
    }

    public void Wake()
    {
        IsAsleep = false;
        GameTime.TimeScale = GameTime.NormalTimeScale;

        // Fade in from black.
        ScreenFader.fadeTime = FadeInFromBlackTime;
        ScreenFader.fadeIn = true;
    }

    void OnFadeInComplete()
    {
        MessageBox.Show("Zzzz...", gameObject);
        GameTime.TimeScale = SleepTimeScale;
    }

    void ShowWakeMessage()
    {
        switch (sleepQualityAtSleep)
        {
            case SleepQualityEnum.POOR:
                MessageBox.ShowForTime("You awake feeling sore after an unpleasant sleep", 2.0f, gameObject);
                break;
            case SleepQualityEnum.OK:
                MessageBox.ShowForTime("You awake feeling sore but refreshed", 2.0f, gameObject);
                break;
            case SleepQualityEnum.GOOD:
                MessageBox.ShowForTime("You awake feeling refreshed", 2.0f, gameObject);
                break;
        }
    }
	
	void Update ()
    {
        // Handle waking up from sleep. 
        if (IsAsleep)
        {
            float gameTimeDelta = Time.deltaTime / 60.0f / 60.0f * GameTime.TimeScale;

            // Count number of hours slept.
            hoursSlept += gameTimeDelta;

            // Perform hourly checks.
            timeSinceLastHour += gameTimeDelta;
            if (timeSinceLastHour >= 1.0f)
            {
                // Chance of being woken by police if in public.
                if (InPublic && 
                    GameTime.TimeOfDayHours > CanBeWokenInPublicFromHour && 
                    GameTime.TimeOfDayHours < CanBeWokenInPublicToHour)
                {
                    var value = Random.Range(0.0f, 1.0f);
                    if (value <= ChanceOfBeingWokenInPublicPerHour)
                    {
                        MessageBox.ShowForTime("You're woken by a police-man saying \"You can't sleep here\"", 2.0f, gameObject);
                        Wake();
                    }                    
                }
                timeSinceLastHour = 0.0f;
            }

            // Wake up in morning.
            if (Mathf.Abs(GameTime.TimeOfDayHours - WakeUpHour) <= gameTimeDelta)
            {
                GameTime.TimeOfDayHours = WakeUpHour;
                ShowWakeMessage();
                Wake();
            }

            // Wake up when max sleep hours is reached.
            if (hoursSlept > MaxSleepHours)
            {
                ShowWakeMessage();
                Wake();
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
