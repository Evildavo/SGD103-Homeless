﻿using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    public enum DayOfTheWeekEnum
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

    [Header("Time scales")]
    public float NormalTimeScale = 100.0f;
    public float AcceleratedTimeScale = 400.0f;
    public float TimeScale = 100.0f;

    [Header("Date and time")]
    public int Day = 1;
    public DayOfTheWeekEnum DayOfTheWeek = DayOfTheWeekEnum.MONDAY;
    [Range(0.0f, 24.0f)]
    public float TimeOfDayHours = 0.0f;
    public float SunriseAtHour = 6.0f;
    [ReadOnly]
    public bool IsNight = false;
    public bool StartWithNormalTimeScale = true;

    // Result for the TimeOfDayHoursDelta function.
    public struct Delta
    {
        public float forward;
        public float backward;
    }
    
    // Returns the number of hours between time-of-day hours a and b.
    // As the 24 hour clock is a loop there are two possible answers, going either forward or backward.
    public Delta TimeOfDayHoursDelta(float a, float b)
    {
        Delta delta;

        // Forward direction.
        if (a == b)
        {
            delta.forward = 0.0f;
        }
        else if (a < b)
        {
            delta.forward = b - a;
        }
        else
        {
            delta.forward = (24.0f - a) + b;
        }

        // Backward direction.
        if (a == b)
        {
            delta.backward = 0.0f;
        }
        else if (a < b)
        {
            delta.backward = a + (24.0f - b);
        }
        else
        {
            delta.backward = a - b;
        }
        return delta;
    }

    // Returns the day of the week that follows the given day.
    public DayOfTheWeekEnum NextDayAfter(DayOfTheWeekEnum day)
    {
        if (day == DayOfTheWeekEnum.SUNDAY)
        {
            return DayOfTheWeekEnum.MONDAY;
        }
        else
        {
            return day + 1;
        }
    }

    // Returns the given day of the week as a short string.
    public string DayOfTheWeekAsShortString(DayOfTheWeekEnum dotw)
    {
        switch (dotw)
        {
            case DayOfTheWeekEnum.MONDAY:
                return "Mon";
            case DayOfTheWeekEnum.TUESDAY:
                return "Tue";
            case DayOfTheWeekEnum.WEDNESDAY:
                return "Wed";
            case DayOfTheWeekEnum.THURSDAY:
                return "Thu";
            case DayOfTheWeekEnum.FRIDAY:
                return "Fri";
            case DayOfTheWeekEnum.SATURDAY:
                return "Sat";
            case DayOfTheWeekEnum.SUNDAY:
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
            Day++;
            DayOfTheWeek = NextDayAfter(DayOfTheWeek);
        }

        // Determine if it's night time.
        IsNight = (
            TimeOfDayHours < SunriseAtHour || 
            TimeOfDayHours > SunriseAtHour + 12.0f);
    }
}
