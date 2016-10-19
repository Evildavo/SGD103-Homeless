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

    public MessageBox MessageBox;
    public GameTime GameTime;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public InventoryItem ResumePrefab;

    public string Name;
    public float ChanceJobAvailablePerDay = 0.05f;
    [Header("Make job available after so many tries (-1 to disable).")]
    public int GuaranteedAvailableAfterDaysChecked = -1;
    
    public bool IsJobAvailableToday = false;
    public JobPositionProfile Job;

    public bool PlayerHasJobHere = false;
    public GameTime.DayOfTheWeekEnum jobStartsAfter;
    public bool workWeekStarted = false;

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

    // Starts the working day.
    public void StartWork()
    {
        Debug.Log("Starting work");
    }

    // Fires the player immediately with the given reason as explanation.
    public void Dismiss(string reason)
    {
        PlayerHasJobHere = false;
        string message = "You have been dismissed from employment. Reason: " + reason;
        MessageBox.ShowForTime(message, 8.0f, gameObject);
    }

    void Start()
    {
        Job.Location = this;
        Job.Calculate();
    }

    void Update()
    {
#if UNITY_EDITOR
        Job.Calculate();
#endif

        // Determine if work has started yet.
        CanWorkNow = false;
        if (PlayerHasJobHere)
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
                    else
                    {
                        Dismiss("Late for work");
                    }
                }
            }
        }
    }

}
