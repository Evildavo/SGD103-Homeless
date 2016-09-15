using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour
{

    public GameTime GameTime;

    void Start()
    {

    }

    void Update()
    {
        // Calculate sun angle.
        var SUNRISE_AT_HOUR = 6.0f;
        var sunXRotationDegrees = (GameTime.TimeOfDayHours - SUNRISE_AT_HOUR) / 24.0f * 360.0f;
        var sunYRotationDegrees = 180.0f;
        transform.rotation =
            Quaternion.AngleAxis(sunXRotationDegrees, Vector3.left) *
            Quaternion.AngleAxis(sunYRotationDegrees, Vector3.up);
    }
}
