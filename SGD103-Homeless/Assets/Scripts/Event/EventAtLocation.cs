using UnityEngine;
using System.Collections;

public class EventAtLocation : MonoBehaviour {
    private float hourAtAttendance;

    public Main Main;

    public GameTime.DayOfTheWeekEnum Day;
    [Header("Note: Does NOT support wrapping (e.g. 11pm to 2am).")]
    public float FromHour;
    public float ToHour;
    public float FadeToBlackTime;
    public float FadeInFromBlackTime;
    public string DuringAttendanceMessage = "Attending...";
    public float AttendanceTimeScale;

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
        Main.GameTime.TimeScale = Main.GameTime.NormalTimeScale;
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
               Input.GetKeyDown("return");
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
        }
        else
        {
            // Determine if open today.
            IsOpen = false;
            if (Day == GameTime.DayOfTheWeek)
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
            Main.GameTime.TimeScale = AttendanceTimeScale;
        }
    }

    void onFadeInComplete()
    {
        Main.MessageBox.ShowNext();
    }

}
