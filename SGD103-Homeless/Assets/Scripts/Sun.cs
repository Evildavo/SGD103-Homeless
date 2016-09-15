using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour
{

    public GameTime GameTime;

    public float SunRiseAtHour = 6.0f;
    public float DeclinationAngleOffset = 30.0f;

    void Start()
    {

    }

    void Update()
    {
        // Calculate sun angle.
        var sunXRotationDegrees = (GameTime.TimeOfDayHours - SunRiseAtHour) / 24.0f * 360.0f;
        var sunYRotationDegrees = 180.0f;
        transform.rotation =
            Quaternion.AngleAxis(sunXRotationDegrees, Vector3.left) *
            Quaternion.AngleAxis(sunYRotationDegrees, Vector3.up);
        transform.Rotate(new Vector3(0.0f, DeclinationAngleOffset, 0.0f));
    }
}
