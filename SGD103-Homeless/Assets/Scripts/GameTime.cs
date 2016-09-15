using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    public float NormalTimeScale = 1.0f;
    public float AcceleratedTimeScale = 1.0f;
    public bool IsTimeAccelerated = false;

    public int Day = 1;
    [Range(0.0f, 24.0f)]
    public float TimeOfDayHours = 0.0f;
    public float TimeScale = 1.0f;
    public bool IsNight = false;
    public float SunriseAtHour = 6.0f;
    
    void Start () {
	
	}
	
	void Update () {
        if (IsTimeAccelerated)
        {
            TimeScale = AcceleratedTimeScale;
        }
        else
        {
            TimeScale = NormalTimeScale;
        }
        TimeOfDayHours += Time.deltaTime / 60.0f / 60.0f * TimeScale;
        if (TimeOfDayHours >= 24.0f)
        {
            TimeOfDayHours = 0.0f;
            Day += 1;
        }

        // Determine if it's night time.
        IsNight = (
            TimeOfDayHours < SunriseAtHour || 
            TimeOfDayHours > SunriseAtHour + 12.0f);
    }
}
