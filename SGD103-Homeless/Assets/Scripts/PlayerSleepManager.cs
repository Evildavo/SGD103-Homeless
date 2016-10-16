using UnityEngine;
using System.Collections;

public class PlayerSleepManager : MonoBehaviour
{
    private SleepQualityEnum sleepQualityAtSleep;
    private float hoursSlept;
    private float timeSinceLastHour;
    private float hoursSinceLastSlept;
    private bool hasSlept = false;
    private SleepItem usingItem;

    public enum WakeReason
    {
        NONE,
        USER_INTERRUPTED,
        WOKEN_BY_POLICE,
        WOKEN_BY_UNCOMFORTABLE_SLEEP,
        WOKEN_BY_SUN,
        WOKEN_FROM_TOO_MUCH_SLEEP
    }

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
    public UI UI;
    public Inventory Inventory;

    public bool HideUIDuringSleep = true;
    public float FadeToBlackTime = 1.5f;
    public float FadeInFromBlackTime = 1.5f;
    public float SleepTimeScale = 12000.0f;
    [Range(0.0f, 24.0f)]
    public float WakeUpMorningHour = 6.5f;
    public float MaxSleepHours = 12.0f;
    public float GoodSleepQualityLevel = 1.0f;
    public float OkSleepQualityLevel = 0.5f;
    public float PoorSleepQualityLevel = 0.15f;
    [Range(0.0f, 1.0f)]
    public float ChanceOfWakingPoorSleepPerHour = 0.15f;
    [Range(0.0f, 1.0f)]
    public float ChanceOfBeingWokenInPublicPerHour = 0.3f;
    [Range(0.0f, 24.0f)]
    public float CanBeWokenInPublicFromHour = 6.5f;
    [Range(0.0f, 24.0f)]
    public float CanBeWokenInPublicToHour = 22.0f;
    public float MinHoursWaitBetweenSleeps = 0.5f;
    [ReadOnly]
    public bool InPublic = false;
    [ReadOnly]
    public SleepQualityEnum SleepQualityHere;
    [ReadOnly]
    public bool IsAsleep = false;
    [ReadOnly]
    public float SleepQuality = 1.0f;
    [ReadOnly]
    public WakeReason LastWakeReason = WakeReason.NONE;

    // Returns the best available sleep item in the player inventory.
    // Returns null if the player doesn't have any items.
    public SleepItem GetBestSleepItem()
    {
        float bestValueSoFar = 0.0f;
        SleepItem bestItemSoFar = null;
        foreach (SleepItem item in Inventory.ItemContainer.GetComponentsInChildren<SleepItem>())
        {
            if (item.ImprovesSleepQualityPercent >= bestValueSoFar)
            {
                bestItemSoFar = item;
                bestValueSoFar = item.ImprovesSleepQualityPercent;
            }
        }
        return bestItemSoFar;
    }

    // Player goes to sleep at the current location.
    // The player won't sleep if the player hasn't waited long enough since last sleeping.
    public void Sleep(SleepItem sleepItem = null)
    {
        if (!IsAsleep)
        {
            if (!hasSlept || hoursSinceLastSlept >= MinHoursWaitBetweenSleeps)
            {
                IsAsleep = true;
                hasSlept = true;
                hoursSlept = 0.0f;
                timeSinceLastHour = 0.0f;
                hoursSinceLastSlept = 0.0f;

                // Determine the base quality of our sleep.
                sleepQualityAtSleep = SleepQualityHere;
                switch (SleepQualityHere)
                {
                    case SleepQualityEnum.POOR:
                        SleepQuality = PoorSleepQualityLevel;
                        break;
                    case SleepQualityEnum.OK:
                        SleepQuality = OkSleepQualityLevel;
                        break;
                    case SleepQualityEnum.GOOD:
                        SleepQuality = GoodSleepQualityLevel;
                        break;
                }

                // Get a bonus from the selected sleeping item.
                if (sleepItem)
                {
                    usingItem = sleepItem;
                    SleepQuality += sleepItem.ImprovesSleepQualityPercent;
                }

                // If no sleep item is available, use the best item the player has.
                else
                {
                    usingItem = GetBestSleepItem();
                    if (usingItem)
                    {
                        SleepQuality += usingItem.ImprovesSleepQualityPercent;
                    }
                }

                // Fade to black.
                ScreenFader.fadeTime = FadeToBlackTime;
                ScreenFader.fadeIn = false;

                // Hide UI.
                if (HideUIDuringSleep)
                {
                    UI.Hide();
                }

                // Show a sleep message and accelerate time once the fade out is complete.
                Invoke("OnFadeInComplete", FadeToBlackTime);
            }
            else
            {
                MessageBox.ShowForTime("You feel too awake to sleep right now", 2.0f, gameObject);
            }  
        }
    }

    public void Wake()
    {
        IsAsleep = false;
        GameTime.TimeScale = GameTime.NormalTimeScale;

        // Show UI.
        UI.Show();

        // Fade in from black.
        ScreenFader.fadeTime = FadeInFromBlackTime;
        ScreenFader.fadeIn = true;
    }

    void OnFadeInComplete()
    {
        if (IsAsleep)
        {
            MessageBox.Show("Zzzz...", gameObject);
            GameTime.TimeScale = SleepTimeScale;
        }
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
        float gameTimeDelta = Time.deltaTime / 60.0f / 60.0f * GameTime.TimeScale;

        // Handle waking up from sleep. 
        if (IsAsleep)
        {

            // Wake up on key or mouse press.
            if (Input.anyKeyDown)
            {
                LastWakeReason = WakeReason.USER_INTERRUPTED;
                ShowWakeMessage();
                Wake();
                return;
            }

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
                        LastWakeReason = WakeReason.WOKEN_BY_POLICE;
                        MessageBox.ShowForTime("You're woken by a police-man saying \"You can't sleep here\"", 2.0f, gameObject);
                        Wake();
                        return;
                    }                    
                }

                // Chance of waking early if sleeping uncomfortably.
                if (sleepQualityAtSleep == SleepQualityEnum.POOR)
                {
                    var value = Random.Range(0.0f, 1.0f);
                    if (value <= ChanceOfWakingPoorSleepPerHour)
                    {
                        LastWakeReason = WakeReason.WOKEN_BY_UNCOMFORTABLE_SLEEP;
                        ShowWakeMessage();
                        Wake();
                        return;
                    }
                }
                timeSinceLastHour = 0.0f;
            }

            // Wake up in morning.
            if (Mathf.Abs(GameTime.TimeOfDayHours - WakeUpMorningHour) <= gameTimeDelta)
            {
                LastWakeReason = WakeReason.WOKEN_BY_SUN;
                GameTime.TimeOfDayHours = WakeUpMorningHour;
                ShowWakeMessage();
                Wake();
                return;
            }

            // Wake up when max sleep hours is reached.
            if (hoursSlept > MaxSleepHours)
            {
                LastWakeReason = WakeReason.WOKEN_FROM_TOO_MUCH_SLEEP;
                ShowWakeMessage();
                Wake();
                return;
            }
        }

        // Determine the sleeping conditions here.
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

            // Keep track of number of hours since we last slept.
            hoursSinceLastSlept += gameTimeDelta;
        }
    }

}
