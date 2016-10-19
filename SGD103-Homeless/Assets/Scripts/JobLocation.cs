using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class JobLocation : MonoBehaviour {
    
    // Represents a job position.
    [System.Serializable]
    public class JobPositionProfile
    {
        public string Role;
        public float PayPerHour;
        public float MinHealthNeededToQualify;
        public float MinMoraleNeededToQualify;
        public float MinClothesCleanlinessToQualify;
        public float ChanceOfSuccessWithoutResume;
        public float ChanceOfSuccessWithResume;
        public GameTime.DayOfTheWeekEnum WorkFromDay;
        public GameTime.DayOfTheWeekEnum WorkToDay;
        [Header("Note: Supports wrapping over (e.g. 11pm to 2am)")]
        public float ShiftFromHour;
        public float ShiftToHour;
        public GameTime.DayOfTheWeekEnum PayDay;
        public float PayTime;
        public float TimeAllowedEarly = 0.5f;
        public float MaxTimeAllowedLate = 0.5f;
        [ReadOnly]
        public JobLocation Location;
        [ReadOnly]
        public int DaysWorkPerWeek;
        [ReadOnly]
        public int HoursWorkPerShift;
        [ReadOnly]
        public int HoursWorkPerWeek;
        [ReadOnly]
        public float PayPerWeek;

        public void Calculate()
        {
            // Calculate the number of days worked per week.
            DaysWorkPerWeek = 1;
            GameTime.DayOfTheWeekEnum dayOfTheWeek = WorkFromDay;
            while (dayOfTheWeek != WorkToDay && DaysWorkPerWeek < 7)
            {
                DaysWorkPerWeek++;
                if (dayOfTheWeek == GameTime.DayOfTheWeekEnum.SUNDAY)
                {
                    dayOfTheWeek = GameTime.DayOfTheWeekEnum.MONDAY;
                }
                else
                {
                    dayOfTheWeek++;
                }
            }

            // Calculate the number of hours worked per week.
            if (ShiftFromHour < ShiftToHour)
            {
                HoursWorkPerShift = Mathf.RoundToInt(ShiftToHour - ShiftFromHour);
            }
            else
            {
                HoursWorkPerShift = Mathf.RoundToInt((24.0f - ShiftFromHour) + ShiftToHour);
            }
            HoursWorkPerWeek = HoursWorkPerShift * DaysWorkPerWeek;

            // Calculate pay per week.
            PayPerWeek = PayPerHour * HoursWorkPerWeek;
        }
    }

    private int dayLastChecked;
    private bool hasChecked = false;
    private int daysChecked = 0;
    private GameTime.DayOfTheWeekEnum jobStartsAfter;
    private bool workWeekStarted = false;
    private float timeAtShiftStart = 0.0f;
    private float payDue = 0.0f;
    private float hoursWorkedThisWeek = 0.0f;

    public MessageBox MessageBox;
    public GameTime GameTime;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public ResumeItem ResumePrefab;
    public JobTrigger JobTrigger;
    public UI UI;
    public ScreenFader ScreenFader;

    public string Name;
    public float ChanceJobAvailablePerDay = 0.05f;
    [Header("Make job available after so many tries (-1 to disable).")]
    public int GuaranteedAvailableAfterDaysChecked = -1;
    public float FadeToBlackTime = 2.0f;
    public float FadeInFromBlackTime = 2.0f;
    public float WorkTimeScale = 8000.0f;

    public bool IsJobAvailableToday = false;
    public JobPositionProfile Job;

    public bool PlayerHasJobHere = false;
    public bool IsPlayerAtWork = false;
    public int LastDayWorked;
    [ReadOnly]
    public bool CanWorkNow = false;

    // Returns a short summary of the work days and times.
    public string GetWorkTimeSummaryShort()
    {
        return GameTime.DayOfTheWeekAsShortString(Job.WorkFromDay) + " to " +
               GameTime.DayOfTheWeekAsShortString(Job.WorkToDay) + ", " +
               GameTime.GetTimeAsString(Job.ShiftFromHour) + " - " +
               GameTime.GetTimeAsString(Job.ShiftToHour);
    }

    // Checks for a job and optionally displays a message if one is available.
    public void CheckForJob(bool showMessage = false)
    {
        // Make sure we don't already have this job.
        if (PlayerHasJobHere)
        {
            IsJobAvailableToday = false;
        }
        else
        {
            // Check if it's a new day (or we've never checked before).
            if (GameTime.Day != dayLastChecked || !hasChecked)
            {
                // Randomly decide if a job is available today.
                float value = Random.Range(0.0f, 1.0f);
                if (value <= ChanceJobAvailablePerDay)
                {
                    IsJobAvailableToday = true;
                }
                hasChecked = true;
                dayLastChecked = GameTime.Day;
                ++daysChecked;
            }

            // Make job available as a one-time kindness to the player after so many tries.
            const int DISABLED = -1;
            if ((GuaranteedAvailableAfterDaysChecked != DISABLED &&
                daysChecked > GuaranteedAvailableAfterDaysChecked) || IsJobAvailableToday)
            {
                IsJobAvailableToday = true;
                GuaranteedAvailableAfterDaysChecked = DISABLED;
            }

            // Show message about the job if it's available.
            if (IsJobAvailableToday && Job != null)
            {
                string message = "A job position is available today as: " + Job.Role + "\n" +
                                 "$" + Job.PayPerHour.ToString("f2") + "/hr, " +
                                 Job.HoursWorkPerWeek + " hours per week.";
                MessageBox.Show(message, gameObject);
            }
        }
    }

    void RejectApplication(string reason)
    {
        MessageBox.Show("Application rejected. " + reason, gameObject);
    }

    // Applies for the job that's available.
    public void ApplyForJob()
    {
        // Check the basic criteria for acceptance.
        bool healthOk = false;
        bool moraleOk = false;
        bool clothesOk = false;
        if (PlayerState.HealthTiredness >= Job.MinHealthNeededToQualify)
        {
            healthOk = true;
        }
        else
        {
            RejectApplication("You seem too unwell to handle the job");
        }
        if (PlayerState.Morale >= Job.MinMoraleNeededToQualify)
        {
            moraleOk = true;
        }
        else
        {
            RejectApplication("You need to have a more positive attitude");
        }
        if (PlayerState.CurrentClothingCleanliness >= Job.MinClothesCleanlinessToQualify)
        {
            clothesOk = true;
        }
        else
        {
            RejectApplication("You should take better care of your appearance");
        }

        // After the basic criteria there's a chance of success. Having a resume guarantees this step.
        if (healthOk && moraleOk && clothesOk)
        {
            bool success = false;
            float value = Random.Range(0.0f, 1.0f);
            if (ResumePrefab && Inventory.HasItem(ResumePrefab))
            {
                success = (value <= Job.ChanceOfSuccessWithResume);

                // Use up an inventory item.
                ResumeItem resume = Inventory.ItemContainer.GetComponentInChildren<ResumeItem>();
                resume.NumUses -= 1;
                if (resume.NumUses == 0)
                {
                    Inventory.RemoveItem(resume);
                }
                Inventory.ShowPreview();
            }
            else
            {
                success = (value <= Job.ChanceOfSuccessWithoutResume);
            }

            // Report final decision.
            if (success)
            {
                string message = "Congratulations! From tomorrow you work " +
                                 GameTime.DayOfTheWeekAsShortString(Job.WorkFromDay) + " to " +
                                 GameTime.DayOfTheWeekAsShortString(Job.WorkToDay) + " from " +
                                 GameTime.GetTimeAsString(Job.ShiftFromHour) + " to " +
                                 GameTime.GetTimeAsString(Job.ShiftToHour) + ". Don't be late!";
                                
                MessageBox.Show(message, gameObject);
                PlayerHasJobHere = true;
                workWeekStarted = false;
                jobStartsAfter = GameTime.DayOfTheWeek;
            }
            else
            {
                RejectApplication("Unfortunately the job is already taken. Try again another time");
            }
        }

        // In any case the job is no longer available today.
        IsJobAvailableToday = false;
    }
    
    // Fires the player immediately with the given reason as explanation.
    public void Dismiss(string reason)
    {
        PlayerHasJobHere = false;
        string message = "You have been dismissed from employment. Reason: " + reason + ".";
        if (payDue > 0.0f)
        {
            message += "Your pay comes to $" + payDue.ToString("f2");
            PlayerState.Money += payDue;
            payDue = 0.0f;
            hoursWorkedThisWeek = 0.0f;
        }
        MessageBox.ShowForTime(message, 8.0f, gameObject);
    }

    void OnFadeOutComplete()
    {
        if (IsPlayerAtWork)
        {
            MessageBox.Show("Working...", gameObject);
            GameTime.TimeScale = WorkTimeScale;
        }
    }

    public void OnTriggerJob()
    {
        // Start work.
        if (!IsPlayerAtWork)
        {
            IsPlayerAtWork = true;
            LastDayWorked = GameTime.Day;
            timeAtShiftStart = GameTime.TimeOfDayHours;

            // Fade to black.
            ScreenFader.fadeTime = FadeToBlackTime;
            ScreenFader.fadeIn = false;
            Invoke("OnFadeOutComplete", FadeToBlackTime);

            // Hide UI.
            UI.Hide();
        }
        JobTrigger.Reset(false);
    }

    public void OnPlayerExitJob()
    {
        if (IsPlayerAtWork)
        {
            UI.Show();
            Dismiss("Leaving work early");
        }
        JobTrigger.Reset(false);
    }

    void checkCanWorkNow()
    {
        // Determine if work is about to start/has started.
        CanWorkNow = false;
        if (PlayerHasJobHere && !IsPlayerAtWork)
        {
            // After the day the player got the job we start the first work week.
            if (!workWeekStarted && GameTime.DayOfTheWeek != jobStartsAfter)
            {
                workWeekStarted = true;
            }

            // Check date and time.
            if (workWeekStarted)
            {
                // Check if today is a work day.
                bool workToday = false;
                GameTime.DayOfTheWeekEnum dotw = Job.WorkFromDay;
                int nDaysChecked = 0;
                while (!workToday && dotw != Job.WorkToDay && nDaysChecked < 7)
                {
                    if (dotw == GameTime.DayOfTheWeek)
                    {
                        workToday = true;
                    }
                    dotw = GameTime.NextDayAfter(dotw);
                    nDaysChecked++;
                }
                if (!workToday && dotw == GameTime.DayOfTheWeek)
                {
                    workToday = true;
                }

                // Check if we're within time.
                GameTime.Delta delta = GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, Job.ShiftFromHour);
                bool earlyOrOnTime = (delta.forward <= delta.backward);
                if (earlyOrOnTime)
                {
                    if (delta.forward <= Job.TimeAllowedEarly)
                    {
                        CanWorkNow = true;
                    }
                }

                // Check if we're late.
                else
                {
                    // A little late is fine.
                    if (delta.backward <= Job.MaxTimeAllowedLate)
                    {
                        CanWorkNow = true;
                    }
                    else if (!IsPlayerAtWork && delta.backward <= Job.MaxTimeAllowedLate + GameTime.GameTimeDelta)
                    {
                        Dismiss("Late for work");
                    }
                }
            }
        }
    }

    void Start()
    {
        Job.Location = this;
        Job.Calculate();

        JobTrigger.RegisterOnTriggerListener(OnTriggerJob);
        JobTrigger.RegisterOnPlayerExitListener(OnPlayerExitJob);
    }

    void Update()
    {
#if UNITY_EDITOR
        Job.Calculate();
#endif
        float gameTimeDelta = GameTime.GameTimeDelta;
        float now = GameTime.TimeOfDayHours;

        // Pay day (and time).
        if (GameTime.DayOfTheWeek == Job.PayDay && 
            Mathf.Abs(GameTime.TimeOfDayHours - Job.PayTime) <= gameTimeDelta &&
            payDue > 0.0f)
        {
            string message = "Work week complete. You worked a total of " +
                             ((int)hoursWorkedThisWeek).ToString() + " hours and have now earned $" +
                             payDue.ToString("f2");
            PlayerState.Money += payDue;
            payDue = 0.0f;
            hoursWorkedThisWeek = 0.0f;
            MessageBox.ShowForTime(message, 10.0f, gameObject);
        }

        if (IsPlayerAtWork)
        {
            // Stop work at the end of shift.
            if (GameTime.TimeOfDayHoursDelta(now, Job.ShiftToHour).forward < gameTimeDelta)
            {
                IsPlayerAtWork = false;
                GameTime.TimeScale = GameTime.NormalTimeScale;

                // Show UI.
                UI.Show();

                // Fade in from black.
                ScreenFader.fadeTime = FadeInFromBlackTime;
                ScreenFader.fadeIn = true;

                // Calculate earnings.
                float hoursWorked = GameTime.TimeOfDayHoursDelta(timeAtShiftStart, Job.ShiftToHour).forward;
                float pay = hoursWorked * Job.PayPerHour;
                payDue += pay;
                hoursWorked = Mathf.Min(hoursWorked, Job.HoursWorkPerShift); // No overpay for starting early.
                hoursWorkedThisWeek += hoursWorked;
                string message = "Work day complete. You worked " +
                                 ((int)hoursWorked).ToString() + " hours and earned $" +
                                 pay.ToString("f2") + " (to be payed on " + 
                                 GameTime.DayOfTheWeekAsShortString(Job.PayDay) + " at " +
                                 GameTime.GetTimeAsString(Job.PayTime) + ")";
                MessageBox.ShowForTime(message, 8.0f, gameObject);
            }
        }
        else
        {
            checkCanWorkNow();
        }        
    }

}
