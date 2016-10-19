using UnityEngine;
using System.Collections;

public class JobLocation : MonoBehaviour {
    private int dayLastChecked;
    private bool hasChecked = false;

    public MessageBox MessageBox;
    public GameTime GameTime;

    public string Name;
    public float ChanceJobAvailablePerDay = 0.05f;

    // Represents a job position.
    [System.Serializable]
    public class JobPositionProfile
    {
        public string Role;
        public float PayPerHour;
        public int HoursPerWeek;

        JobPositionProfile(string role, float payPerHour, int hoursPerWeek)
        {
            Role = role;
            PayPerHour = payPerHour;
            HoursPerWeek = hoursPerWeek;
        }
    }

    public bool IsJobAvailableToday = false;
    public JobPositionProfile Job = null;

    // Checks for a job and optionally displays a message if one is available.
    public void CheckForJob(bool showMessage = false)
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
        }

        // Show message about the job if it's available.
        if (IsJobAvailableToday && Job != null)
        {
            string message = "A job position is available today as: " + Job.Role + "\n" +
                             "$" + Job.PayPerHour.ToString("f2") + "/hr, " +
                             Job.HoursPerWeek + " hours per week.\n";
            MessageBox.Show(message, gameObject);
        }
    }

    // Applies for the job that's available.
    public void ApplyForJob()
    {
        Debug.Log("Applying for job");
    }
}
