using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    float timeOwed;
    float acceleratedTimeSpeed;

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
    [Header("Default speed of time while doing an on-going action like reading")]
    public float AcceleratedTimeScale = 400.0f;
    [Header("Speed time skips forward after doing a one-time action that costs time")]
    public float TimeSkipSpeed = 1000.0f;
    [ReadOnly]
    public float TimeScale = 100.0f;
    [ReadOnly]
    public bool IsTimeSkipping;
    [ReadOnly]
    public bool IsTimeAccelerated;

    [Header("Date and time")]
    public int Day = 1;
    public DayOfTheWeekEnum DayOfTheWeek = DayOfTheWeekEnum.MONDAY;
    [Range(0.0f, 24.0f)]
    public float TimeOfDayHours = 0.0f;
    public float SunriseAtHour = 6.0f;
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

    // Spends the given amount of time, causing an accelerated skip in game-time.
    public void SpendTime(float hours)
    {
        if (hours > 0.0f)
        {
            timeOwed += hours;
            IsTimeSkipping = true;
        }
    }

    // Sets the time acceleration speed. 
    public void AccelerateTime(float speed)
    {
        acceleratedTimeSpeed = speed;
        IsTimeAccelerated = true;
    }

    // Sets the time acceleration scale to the default accelerated time speed. 
    public void AccelerateTime()
    {
        AccelerateTime(AcceleratedTimeScale);
    }

    // Resets time acceleration to normal (doesn't stop acceleration from time-skips though).
    public void ResetToNormalTime()
    {
        IsTimeAccelerated = false;
    }
    	
	void Update ()
    {
        // Increase time scale if time accelerated and/or while skipping time.
        if (timeOwed != 0.0f)
        {
            if (!IsTimeAccelerated)
            {
                TimeScale = TimeSkipSpeed;
            }
            else
            {
                TimeScale = acceleratedTimeSpeed + TimeSkipSpeed;
            }
        }
        else
        {
            if (!IsTimeAccelerated)
            {
                TimeScale = NormalTimeScale;
            }
            else
            {
                TimeScale = acceleratedTimeSpeed;
            }
        }
        
        // Calculate game time delta.
        GameTimeDelta = Time.deltaTime / 60.0f / 60.0f * TimeScale;
        
        // Decrement time owed.
        if (timeOwed >= GameTimeDelta)
        {
            timeOwed -= GameTimeDelta;
        }
        else if (timeOwed != 0.0f)
        {
            // Apply last bit of time owed to game time.
            GameTimeDelta = timeOwed;
            timeOwed = 0.0f;
            IsTimeSkipping = false;
        }

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
