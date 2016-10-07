using UnityEngine;
using System.Collections;

public class BackgroundCar : MonoBehaviour {
    private bool isActive = true;

    public GameTime GameTime;

    public float SpeedKmPerHour = 60.0f;
    public Zone WrapFrom;
    public Zone WrapTo;

    [Range(0.0f, 24.0f)]
    public float ActiveFromHour = 7.0f;
    [Range(0.0f, 24.0f)]
    public float ActiveToHour = 18.0f;

    void Start() {

        // Hide if we're not in active hours.
        if (GameTime.TimeOfDayHours < ActiveFromHour || GameTime.TimeOfDayHours > ActiveToHour)
        {
            GetComponent<Renderer>().enabled = false;
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
            if (!isActive && GameTime.TimeOfDayHours >= ActiveFromHour && GameTime.TimeOfDayHours <= ActiveToHour)
            {
                GetComponent<Renderer>().enabled = true;
                isActive = true;
            }
            // Become inactive if we've exited active hours.
            else if (isActive && (GameTime.TimeOfDayHours < ActiveFromHour || GameTime.TimeOfDayHours > ActiveToHour))
            {
                GetComponent<Renderer>().enabled = false;
                isActive = false;
            }
        }
    }
}
