using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    public enum DayOfTheWeek
    {
        NONE,
        MONDAY,
        TUESDAY,
        WEDNESDAY,
        THURSDAY,
        FRIDAY,
        SATURDAY,
        SUNDAY
    }

    [Header("Game time")]
    public float NormalTimeScale = 100.0f;
    public float AcceleratedTimeScale = 400.0f;
    public float TimeScale = 100.0f;

    [Header("Time-of-day")]
    public int Day = 1;
    [Range(0.0f, 24.0f)]
    public float TimeOfDayHours = 0.0f;
    public float SunriseAtHour = 6.0f;
    [ReadOnly]
    public bool IsNight = false;
    public bool StartWithNormalTimeScale = true;

    // Returns the given day of the week as a short string.
    public string DayOfTheWeekAsShortString(DayOfTheWeek dotw)
    {
        switch (dotw)
        {
            case DayOfTheWeek.MONDAY:
                return "Mon";
            case DayOfTheWeek.TUESDAY:
                return "Tue";
            case DayOfTheWeek.WEDNESDAY:
                return "Wed";
            case DayOfTheWeek.THURSDAY:
                return "Thu";
            case DayOfTheWeek.FRIDAY:
                return "Fri";
            case DayOfTheWeek.SATURDAY:
                return "Sat";
            case DayOfTheWeek.SUNDAY:
                return "Sun";
        }
        return "";
    }

    // Returns the given time as a string in the format "11:34 pm".
    public string GetTimeAsString(float time)
    {
        int hour = (int)time;
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
        int minutes = (int)((time - Mathf.Floor(time)) * 60.0f);
        return hour.ToString() + ":" + minutes.ToString().PadLeft(2, '0') + " " + amPmLabel.ToUpper();
    }

    // Returns the current time as a string in the format "11:34 pm".
    public string GetTimeAsString()
    {
        return GetTimeAsString(TimeOfDayHours);
    }

    void Start ()
    {
        if (StartWithNormalTimeScale)
        {
            TimeScale = NormalTimeScale;
        }
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
