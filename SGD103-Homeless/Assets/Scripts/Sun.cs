using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {

    public GameTime GameTime;
    
	void Start () {
	
	}
	
	void Update () {
        var SUNRISE_AT_HOUR = 6.0f;

        var sunXRotationDegrees = (GameTime.TimeOfDayHours - SUNRISE_AT_HOUR) / 24.0f * 360.0f;
        transform.rotation = Quaternion.AngleAxis(sunXRotationDegrees, Vector3.left);
    }
}
