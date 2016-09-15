using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour
{

    public GameTime GameTime;
    
    public float DeclinationAngleOffset = 30.0f;
    public Gradient ColourGradient;
    public Gradient IntensityGradient;

    void Start()
    {

    }

    void Update()
    {
        // Adjust sun light based on time.
        Light light = GetComponent<Light>();
        light.color = ColourGradient.Evaluate(GameTime.TimeOfDayHours / 24.0f);
        light.intensity = IntensityGradient.Evaluate(GameTime.TimeOfDayHours / 24.0f).grayscale;

        // Calculate sun angle.
        var sunXRotationDegrees = (GameTime.TimeOfDayHours - GameTime.SunriseAtHour) / 24.0f * 360.0f;
        var sunYRotationDegrees = 180.0f;
        transform.rotation =
            Quaternion.AngleAxis(sunXRotationDegrees, Vector3.left) *
            Quaternion.AngleAxis(sunYRotationDegrees, Vector3.up);
        transform.Rotate(new Vector3(0.0f, DeclinationAngleOffset, 0.0f));
        
        DynamicGI.UpdateEnvironment();
    }
}
