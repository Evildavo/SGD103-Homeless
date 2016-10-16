using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    [Header("Game time")]
    public float NormalTimeScale = 1.0f;
    public float AcceleratedTimeScale = 1.0f;
    public float TimeScale = 1.0f;

    [Header("Time-of-day")]
    public int Day = 1;
    [Range(0.0f, 24.0f)]
    public float TimeOfDayHours = 0.0f;
    public float SunriseAtHour = 6.0f;
    [ReadOnly]
    public bool IsNight = false;
    
    // Returns the time as a string in the format "11:34 pm".
    public string GetTimeAsString()
    {
        int hour = (int)TimeOfDayHours;
        string amPmLabel;
        if (hour < 1)
        {
            hour = 12;
            amPmLabel = "am";
        }
        else if (hour < 12)
        {
            amPmLabel = "am";
        }
        else if (hour < 13)
        {
            hour = 12;
            amPmLabel = "pm";
        }
        else
        {
            hour -= 12;
            amPmLabel = "pm";
        }
        int minutes = (int)((TimeOfDayHours - Mathf.Floor(TimeOfDayHours)) * 60.0f);
        return hour.ToString() + ":" + minutes.ToString().PadLeft(2, '0') + " " + amPmLabel.ToUpper();
    }

    void Start () {
	
	}
	
	void Update ()
    {
        // Increment game-time.
        TimeOfDayHours += Time.deltaTime / 60.0f / 60.0f * TimeScale;
        if (TimeOfDayHours >= 24.0f)
        {
            TimeOfDayHours = 0.0f;
            Day += 1;
        }

        // Determine if it's night time.
        IsNight = (
            TimeOfDayHours < SunriseAtHour || 
            TimeOfDayHours > SunriseAtHour + 12.0f);
    }
}
