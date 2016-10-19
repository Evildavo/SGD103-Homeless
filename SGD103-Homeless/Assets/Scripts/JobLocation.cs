using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class JobLocation : MonoBehaviour {
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
        public GameTime.DayOfTheWeek WorkDay1;
        public GameTime.DayOfTheWeek WorkDay2;
        [Header("Note: Supports wrapping over (e.g. 11pm to 2am)")]
        public float ShiftFromHour;
        public float ShiftToHour;
        [ReadOnly]
        public JobLocation Location;
        [ReadOnly]
        public int HoursWorkPerWeek;
        [ReadOnly]
        public float PayPerWeek;

        public int GetHoursPerShift()
        {
            if (ShiftFromHour < ShiftToHour)
            {
                return Mathf.RoundToInt(ShiftToHour - ShiftFromHour);
            }
            else
            {
                return Mathf.RoundToInt((24.0f - ShiftFromHour) + ShiftToHour);
            }
        }
    }

    public bool IsJobAvailableToday = false;
    public JobPositionProfile Job;

    // Checks for a job and optionally displays a message if one is available.
    public void CheckForJob(bool showMessage = false)
    {
        // Make sure we don't already have this job.
        foreach (JobPositionProfile job in PlayerState.Jobs)
        {
            if (job == Job)
            {
                IsJobAvailableToday = false;
                return;
            }
        }

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
                             Job.GetHoursPerShift() + " hours per week.";
            MessageBox.Show(message, gameObject);
        }
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
            MessageBox.Show(
                "Application rejected. You seem too unwell to handle the job", gameObject);
        }
        if (PlayerState.Morale >= Job.MinMoraleNeededToQualify)
        {
            moraleOk = true;
        }
        else
        {
            MessageBox.Show(
                "Application rejected. You need to have a more positive attitude", gameObject);
        }
        if (PlayerState.CurrentClothingCleanliness >= Job.MinClothesCleanlinessToQualify)
        {
            clothesOk = true;
        }
        else
        {
            MessageBox.Show(
                "Application rejected. You should take better care of your appearance", gameObject);
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
                PlayerState.Jobs.Add(Job);
                MessageBox.Show(
                    "Congratulations! From tomorrow you work " + 
                    GameTime.DayOfTheWeekAsShortString(Job.WorkDay1) + " and " +
                    GameTime.DayOfTheWeekAsShortString(Job.WorkDay2) + " from " +
                    GameTime.GetTimeAsString(Job.ShiftFromHour) + " to " +
                    GameTime.GetTimeAsString(Job.ShiftToHour) + ". Don't be late!", gameObject);
            }
            else
            {
                MessageBox.Show(
                    "Application rejected. Unfortunately the job is already taken. Try again another time", gameObject);
            }
        }

        // In any case the job is no longer available today.
        IsJobAvailableToday = false;
    }

    void Start()
    {
        Job.Location = this;
    }

    void Update()
    {
        // Calculate the number of hours worked per week.
        int daysWorked = 0;
        if (Job.WorkDay1 != GameTime.DayOfTheWeek.NONE)
        {
            daysWorked++;
        }
        if (Job.WorkDay2 != GameTime.DayOfTheWeek.NONE)
        {
            daysWorked++;
        }
        Job.HoursWorkPerWeek = Job.GetHoursPerShift() * daysWorked;

        // Calculate pay per week.
        Job.PayPerWeek = Job.PayPerHour * Job.HoursWorkPerWeek;
    }
}
