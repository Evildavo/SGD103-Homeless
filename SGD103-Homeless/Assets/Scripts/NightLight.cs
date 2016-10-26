using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NightLight : MonoBehaviour {
    public float switchDelay;
    public bool isSwitching;
    public float hourAtSwitch;

    public Main Main;

    [Header("Supports wrapping around (e.g. 11pm to 2am)")]
    public float FromHour = 17.0f;
    public float ToHour = 5.0f;
    public float MaxRandomDelaySwitchingHours = 0.25f;

    [ReadOnly]
    public bool IsInHour;

    void Start()
    {
        // Disable initially.
        GetComponent<Light>().enabled = false;
    }

    void Update()
    {
        Light light = GetComponent<Light>();

        // Determine if we're within hour.
        var GameTime = Main.GameTime;
        if (FromHour < ToHour)
        {
            IsInHour = (GameTime.TimeOfDayHours >= FromHour && 
                        GameTime.TimeOfDayHours <= ToHour);
        }
        else
        {
            IsInHour = (GameTime.TimeOfDayHours >= FromHour ||
                        GameTime.TimeOfDayHours <= ToHour);
        }

        // Start switching lights on/off depending on the hour.
        if (!isSwitching && 
            ((IsInHour && !light.enabled) || (!IsInHour && light.enabled)))
        {
            isSwitching = true;
            hourAtSwitch = GameTime.TimeOfDayHours;
            switchDelay = Random.Range(0.0f, MaxRandomDelaySwitchingHours);
        }

        // When finished switching turn the light on/off.
        if (isSwitching && 
            GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, hourAtSwitch).shortest >= switchDelay)
        {
            light.enabled = !light.enabled;
            isSwitching = false;
        }
    }

}
