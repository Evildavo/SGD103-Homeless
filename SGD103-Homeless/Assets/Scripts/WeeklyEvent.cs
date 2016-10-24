using UnityEngine;
using System.Collections;

public class WeeklyEvent : MonoBehaviour {

    public Main Main;
    public GameTime.DayOfTheWeekEnum Day;
    [Header("Note: Does NOT support wrapping (e.g. 11pm to 2am).")]
    public float FromHour;
    public float ToHour;

    [ReadOnly]
    public bool IsOpen;

    // Fades the screen out
    public void Attend(string message)
    {

    }

    void Update()
    {
        // Determine if open today.
        IsOpen = false;
        if (Day == Main.GameTime.DayOfTheWeek)
        {
            // Determine if open now.
            if (Main.GameTime.TimeOfDayHours > FromHour &&
                Main.GameTime.TimeOfDayHours < ToHour)
            {
                IsOpen = true;
            }
        }
    }

}
