using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Sun : MonoBehaviour
{
    private float lastTime;

    public Main Main;
    
    public float DeclinationAngleOffset = 30.0f;
    public Gradient ColourGradient;
    public Gradient IntensityGradient;
    public Gradient SkyGradient;
    public Gradient EquatorGradient;
        
    void Update()
    {
        var GameTime = Main.GameTime;

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

        // Update the ambient sky lighting.
        RenderSettings.ambientSkyColor = SkyGradient.Evaluate(GameTime.TimeOfDayHours / 24.0f);
        RenderSettings.ambientEquatorColor = EquatorGradient.Evaluate(GameTime.TimeOfDayHours / 24.0f);
    }
}
