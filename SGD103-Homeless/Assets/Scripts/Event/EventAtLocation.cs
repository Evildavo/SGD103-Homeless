using UnityEngine;
using System.Collections;

public class EventAtLocation : MonoBehaviour {
    private float hourAtAttendance;

    public Main Main;

    public bool AvailableEveryDay = false;
    public GameTime.DayOfTheWeekEnum Day;
    [Header("Note: Does NOT support wrapping (e.g. 11pm to 2am).")]
    public float FromHour = 0.0f;
    public float ToHour = 24.0f;
    public float FadeToBlackTime = 2.0f;
    public float FadeInFromBlackTime = 2.0f;
    public string DuringAttendanceMessage = "Attending...";
    public float AttendanceTimeScale = 1.0f;
    [Header("Leave as zero to stay until the event closes.")]
    public float DurationPerVisitHours = 0.0f;

    [Space(10)]
    public bool IsCurrentlyAttending = false;
    [ReadOnly]
    public bool IsOpen;
    [ReadOnly]
    public float DurationHours;


    // The screen fades to black to represent the user attending the event.
    public void Attend()
    {
        IsCurrentlyAttending = true;
        hourAtAttendance = Main.GameTime.TimeOfDayHours;

        // Fade to black.
        Main.ScreenFader.fadeTime = FadeToBlackTime;
        Main.ScreenFader.fadeIn = false;
        Invoke("OnFadeOutComplete", FadeToBlackTime);

        // Hide UI.
        Main.UI.Hide();

        OnPlayerAttends();
    }

    // Leaves the event.
    public void Leave()
    {
        IsCurrentlyAttending = false;
        Main.GameTime.ResetToNormalTime();
        Main.MessageBox.Hide();

        // Show UI.
        Main.UI.Show();

        // Fade in from black.
        Main.ScreenFader.fadeTime = FadeInFromBlackTime;
        Main.ScreenFader.fadeIn = true;

        Invoke("onFadeInComplete", FadeInFromBlackTime);
        OnPlayerLeaves();
    }

    public bool ExitPressed()
    {
        return Input.GetButtonDown("Secondary") ||
               Input.GetKeyDown("e") ||
               Input.GetKeyDown("enter") ||
               Input.GetKeyDown("return") ||
               Input.GetKeyDown("space");
    }

    // Override to do something when the player attends the event.
    protected virtual void OnPlayerAttends() { }

    // Override to do something when the event ends while the player was attending it.
    protected virtual void OnPlayerLeaves() { }
    
    // Call from derived.
    protected void Update ()
    {
        // Update duration.
        DurationHours = Mathf.Abs(FromHour - ToHour);

        var GameTime = Main.GameTime;
        if (IsCurrentlyAttending)
        {
            // On user input, leave.
            if (ExitPressed())
            {
                Leave();
            }

            // At the end of the event, leave.
            if (GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, ToHour).shortest <= GameTime.GameTimeDelta)
            {
                Leave();
            }

            // Force the player to leave if they've stayed longer than the max time.
            if (DurationPerVisitHours != 0 &&
                GameTime.TimeOfDayHoursDelta(hourAtAttendance, GameTime.TimeOfDayHours).shortest > DurationPerVisitHours)
            {
                Leave();
            }
        }
        else
        {
            // Determine if open today.
            IsOpen = false;
            if (Day == GameTime.DayOfTheWeek || AvailableEveryDay)
            {
                // Determine if open now.
                if (GameTime.TimeOfDayHours > FromHour &&
                    GameTime.TimeOfDayHours < ToHour)
                {
                    IsOpen = true;
                }
            }
        }
    }

    void OnFadeOutComplete()
    {
        if (IsCurrentlyAttending)
        {
            // Show attendance message and accelerate time.
            if (DuringAttendanceMessage != "")
            {
                Main.MessageBox.Show(DuringAttendanceMessage, gameObject);
            }
            Main.GameTime.AccelerateTime(AttendanceTimeScale);
        }
    }

    void onFadeInComplete()
    {
        Main.MessageBox.ShowNext();
    }

}
