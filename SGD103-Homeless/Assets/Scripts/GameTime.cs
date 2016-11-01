using UnityEngine;
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
    public bool StartWithNormalTimeScale = true;
    [ReadOnly]
    public bool IsNight = false;
    [ReadOnly]
    public float GameTimeDelta = 0.0f;

    // Result for the TimeOfDayHoursDelta function.
    public struct Delta
    {
        public float forward;
        public float backward;

        // The shortest of either forward or backward.
        public float shortest;
    }
        
    // Returns the number of hours between time-of-day hours a and b.
    // As the 24 hour clock is a loop there are two possible answers, going either forward or backward.
    public static Delta TimeOfDayHoursDelta(float a, float b)
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

        // Shortest.
        delta.shortest = delta.forward < delta.backward ? delta.forward : delta.backward;
        return delta;
    }

    // Returns the day of the week that follows the given day.
    public static DayOfTheWeekEnum NextDayAfter(DayOfTheWeekEnum day)
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
    public static string DayOfTheWeekAsShortString(DayOfTheWeekEnum dotw)
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

    // Returns the given day of the week as a string.
    public static string DayOfTheWeekAsString(DayOfTheWeekEnum dotw)
    {
        switch (dotw)
        {
            case DayOfTheWeekEnum.MONDAY:
                return "Monday";
            case DayOfTheWeekEnum.TUESDAY:
                return "Tuesday";
            case DayOfTheWeekEnum.WEDNESDAY:
                return "Wednesday";
            case DayOfTheWeekEnum.THURSDAY:
                return "Thursday";
            case DayOfTheWeekEnum.FRIDAY:
                return "Friday";
            case DayOfTheWeekEnum.SATURDAY:
                return "Saturday";
            case DayOfTheWeekEnum.SUNDAY:
                return "Sunday";
        }
        return "";
    }

    // Returns the given time as a string in the format "11:34 pm".
    public static string GetTimeAsString(float time)
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

    // Spends the given amount of time, causing an accelerated jump in game-time.
    public void SpendTime(float hours)
    {
        Debug.Log("Spent: " + hours);
        
        TimeOfDayHours += hours;
        if (TimeOfDayHours >= 24.0f)
        {
            TimeOfDayHours = TimeOfDayHours - 24.0f;
            Day++;
            DayOfTheWeek = NextDayAfter(DayOfTheWeek);
        }
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
        // Calculate game time delta.
        GameTimeDelta = Time.deltaTime / 60.0f / 60.0f * TimeScale;

        // Increment game-time.
        TimeOfDayHours += GameTimeDelta;
        if (TimeOfDayHours >= 24.0f)
        {
            TimeOfDayHours = TimeOfDayHours - 24.0f;
            Day++;
            DayOfTheWeek = NextDayAfter(DayOfTheWeek);
        }

        // Determine if it's night time.
        IsNight = (
            TimeOfDayHours < SunriseAtHour || 
            TimeOfDayHours > SunriseAtHour + 12.0f);
    }
}
