using UnityEngine;
using System.Collections;

public class WeeklyEvent : EventAtLocation {

    public Main Main;
    public GameTime.DayOfTheWeekEnum Day;
    [Header("Note: Does NOT support wrapping (e.g. 11pm to 2am).")]
    public float FromHour;
    public float ToHour;

    [ReadOnly]
    public bool IsOpen;
    
    // Call from derived.
    protected new void Start()
    {
        base.Start();
    }

    // Call from derived.
    protected new void Update()
    {
        base.Update();

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
