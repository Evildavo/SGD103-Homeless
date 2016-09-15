using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    public float NormalTimeScale = 1.0f;
    public float AcceleratedTimeScale = 1.0f;
    public bool IsTimeAccelerated = false;

    [Range(0.0f, 24.0f)]
    public float TimeOfDayHours = 0.0f;
    public float TimeScale = 1.0f;
    
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
    }
}
