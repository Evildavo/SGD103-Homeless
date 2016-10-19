using UnityEngine;
using System.Collections;

public class JobLocation : MonoBehaviour {

    public MessageBox MessageBox;

    public string Name;

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
