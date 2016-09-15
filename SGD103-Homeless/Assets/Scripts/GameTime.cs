using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    // Settings.
    public float NormalTimeScale = 1.0f;
    public float AcceleratedTimeScale = 1.0f;
    public bool IsTimeAccelerated = false;
    public float TimeScale = 1.0f;
    
    void Start () {
	
	}
	
	void Update () {
        if (IsTimeAccelerated) { TimeScale = AcceleratedTimeScale; }
        else { TimeScale = NormalTimeScale; }
    }
}
