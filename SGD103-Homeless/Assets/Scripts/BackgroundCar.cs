using UnityEngine;
using System.Collections;

public class BackgroundCar : MonoBehaviour {
    
    public enum ActivePeriods
    {
        PEAK_HOURS,      // < From 8am to 10am and 4pm to 7pm, inclusive.
        REGULAR_HOURS,   // < From 8am to 7pm, inclusive.
        MOST_HOURS,      // < From 5am to 12am midnight, inclusive.
        ALL_HOURS        // < All hours.
    }

    public GameTime GameTime;

    public float SpeedKmPerHour = 60.0f;
    public Zone WrapFrom;
    public Zone WrapTo;
    public ActivePeriods ActivePeriod = ActivePeriods.REGULAR_HOURS;

    [ReadOnly]
    public bool isActive = true;

    // Returns true if the given time in hours is within the active hours for this vehicle.
    bool withinActiveHours(float time)
    {
        int t = (int)time;
        switch (ActivePeriod)
        {
            case ActivePeriods.PEAK_HOURS:
                if ((t >= 8 && t <= 10) || (t >= 16 && t <= 19))
                {
                    return true;
                }
                break;
            case ActivePeriods.REGULAR_HOURS:
                if (t >= 8 && t <= 19)
                {
                    return true;
                }
                break;
            case ActivePeriods.MOST_HOURS:
                if (t >= 5 || t == 0)
                {
                    return true;
                }
                break;
            case ActivePeriods.ALL_HOURS:
                return true;
        }
        return false;
    }

    void show()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = true;
        }
        foreach (Light light in GetComponentsInChildren<Light>(true))
        {
            light.enabled = true;
        }
    }

    void hide()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = false;
        }
        foreach (Light light in GetComponentsInChildren<Light>(true))
        {
            light.enabled = false;
        }
    }

    void Start()
    {
        // The main object is a shell for editor purposes with the actual mesh as a child object.
        GetComponent<Renderer>().enabled = false;

        // Hide if we're not in active hours.
        if (!withinActiveHours(GameTime.TimeOfDayHours))
        {
            hide();
            isActive = false;
        }
	}
	
	void Update ()
    {
        // Move forward.
        const float METERS_PER_KM = 1000.0f;
        const float MINUTES_PER_HOUR = 60.0f;
        const float SECONDS_PER_MINUTE = 60.0f;
        float speedMetersPerSecond = SpeedKmPerHour * METERS_PER_KM / MINUTES_PER_HOUR / SECONDS_PER_MINUTE;
        transform.Translate(new Vector3(speedMetersPerSecond, 0.0f, 0.0f) * Time.deltaTime);
	}
    
    public void OnTriggerEnter(Collider other)
    {
        // Wrap over when running out of road.
        if (other.gameObject == WrapFrom.gameObject)
        {
            Vector3 wrapFromDelta = transform.position - WrapFrom.transform.position;
            transform.position = WrapTo.transform.position + wrapFromDelta;
            
            // Become active if we've entered active hours.
            if (!isActive && withinActiveHours(GameTime.TimeOfDayHours))
            {
                show();
                isActive = true;
            }
            // Become inactive if we've exited active hours.
            else if (isActive && !withinActiveHours(GameTime.TimeOfDayHours))
            {
                hide();
                isActive = false;
            }
        }
    }
}
