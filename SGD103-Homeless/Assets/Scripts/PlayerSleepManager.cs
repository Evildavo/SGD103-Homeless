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

    public Main Main;

    [Header("The order of zones in the container determines their order", order=0)]
    [Space(-10, order=1)]
    [Header("of effect when they overlap (last overrides first).", order=2)]
    public Transform SleepZoneContainer;

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
        foreach (SleepItem item in Main.Inventory.ItemContainer.GetComponentsInChildren<SleepItem>())
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
    // If not sleeping rough the sleep is considered good. 
    // sleepQualityFactor can be used to adjust sleep quality.
    public void Sleep(SleepItem sleepItem = null, bool sleepingRough = true, float sleepQualityFactor = 0.0f)
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
                Main.UI.EnableModalMode();

                // Determine the base quality of our sleep.
                if (sleepingRough)
                {
                    sleepQualityAtSleep = SleepQualityHere;
                    switch (SleepQualityHere)
                    {
                        case SleepQualityEnum.POOR:
                            SleepQuality = PoorSleepQualityLevel * sleepQualityFactor;
                            break;
                        case SleepQualityEnum.OK:
                            SleepQuality = OkSleepQualityLevel * sleepQualityFactor;
                            break;
                        case SleepQualityEnum.GOOD:
                            SleepQuality = GoodSleepQualityLevel * sleepQualityFactor;
                            break;
                    }
                }
                else
                {
                    sleepQualityAtSleep = SleepQualityEnum.GOOD;
                    SleepQuality = sleepQualityFactor;
                }

                // Try to use sleep item if sleeping rough.
                if (sleepingRough)
                {
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
                }

                // Fade to black.
                Main.ScreenFader.fadeTime = FadeToBlackTime;
                Main.ScreenFader.fadeIn = false;
                
                // Hide UI.
                if (HideUIDuringSleep)
                {
                    Main.UI.Hide();
                }

                // Show a sleep message and accelerate time once the fade out is complete.
                Invoke("OnFadeInComplete", FadeToBlackTime);
            }
            else
            {
                Main.MessageBox.ShowForTime("You feel too awake to sleep right now", 2.0f, gameObject);
            }  
        }
    }

    public void Wake()
    {
        IsAsleep = false;
        Main.GameTime.TimeScale = Main.GameTime.NormalTimeScale;
        Main.UI.DisableModalMode();

        // Show UI.
        Main.UI.Show();

        // Fade in from black.
        Main.ScreenFader.fadeTime = FadeInFromBlackTime;
        Main.ScreenFader.fadeIn = true;
    }

    void OnFadeInComplete()
    {
        if (IsAsleep)
        {
            Main.MessageBox.Show("Zzzz...", gameObject);
            Main.GameTime.TimeScale = SleepTimeScale;
        }
    }

    void ShowWakeMessage()
    {
        switch (sleepQualityAtSleep)
        {
            case SleepQualityEnum.POOR:
                Main.MessageBox.ShowForTime("You awake feeling sore after an unpleasant sleep", 2.0f, gameObject);
                break;
            case SleepQualityEnum.OK:
                Main.MessageBox.ShowForTime("You awake feeling sore but refreshed", 2.0f, gameObject);
                break;
            case SleepQualityEnum.GOOD:
                Main.MessageBox.ShowForTime("You awake feeling refreshed", 2.0f, gameObject);
                break;
        }
    }
    
    bool exitPressed()
    {
        return Input.GetButtonDown("Secondary") ||
               Input.GetKeyDown("e") ||
               Input.GetKeyDown("enter") ||
               Input.GetKeyDown("return");
    }

    void Update ()
    {
        float gameTimeDelta = Main.GameTime.GameTimeDelta;

        // Handle waking up from sleep. 
        if (IsAsleep)
        {

            // Wake up on key or mouse press.
            if (exitPressed())
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
                    Main.GameTime.TimeOfDayHours > CanBeWokenInPublicFromHour &&
                    Main.GameTime.TimeOfDayHours < CanBeWokenInPublicToHour)
                {
                    var value = Random.Range(0.0f, 1.0f);
                    if (value <= ChanceOfBeingWokenInPublicPerHour)
                    {
                        LastWakeReason = WakeReason.WOKEN_BY_POLICE;
                        Main.MessageBox.ShowForTime("You're woken by a police-man saying \"You can't sleep here\"", 2.0f, gameObject);
                        Debug.Log("Waking from police");
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
            if (Mathf.Abs(Main.GameTime.TimeOfDayHours - WakeUpMorningHour) <= gameTimeDelta)
            {
                LastWakeReason = WakeReason.WOKEN_BY_SUN;
                Main.GameTime.TimeOfDayHours = WakeUpMorningHour;
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
            if (SleepZoneContainer)
            {
                PublicZone[] publicZones = SleepZoneContainer.GetComponentsInChildren<PublicZone>();
                foreach (PublicZone zone in publicZones)
                {
                    if (zone.PlayerIsInside)
                    {
                        InPublic = true;
                        break;
                    }
                }
            }

            // Determine the quality of sleep here based on zones the player is in.
            SleepQualityHere = SleepQualityEnum.POOR;
            if (SleepZoneContainer)
            {
                SleepZone[] sleepZones = SleepZoneContainer.GetComponentsInChildren<SleepZone>();
                foreach (SleepZone zone in sleepZones)
                {
                    if (zone.PlayerIsInside)
                    {
                        SleepQualityHere = zone.SleepQuality;
                    }
                }
            }

            // Keep track of number of hours since we last slept.
            hoursSinceLastSlept += gameTimeDelta;
        }
    }

}
