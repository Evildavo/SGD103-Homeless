using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NightLight : MonoBehaviour {
    float switchDelay;
    bool isSwitching;
    float hourAtSwitch;
    bool lit;

    public Main Main;

    [Header("Supports wrapping around (e.g. 11pm to 2am)")]
    public float FromHour = 17.0f;
    public float ToHour = 5.0f;
    public float MaxRandomDelaySwitchingHours = 0.25f;
    [Header("Searches for meshes with the lit material")]
    public Material LitMaterial;
    public Material UnlitMaterial;

    [ReadOnly]
    public bool IsInHour;

    void Start()
    {
        Light light = GetComponentInChildren<Light>();
        if (light)
        {
            lit = GetComponentInChildren<Light>().enabled;
        }
        else
        {
            lit = false;
        }
    }

    void Update()
    {
        // Determine if we're within hour.
        var GameTime = Main.GameTime;
        if (FromHour < ToHour)
        {
            IsInHour = (GameTime.TimeOfDayHours >= FromHour && 
                        GameTime.TimeOfDayHours <= ToHour);
        }
        else
        {
            IsInHour = (GameTime.TimeOfDayHours >= FromHour ||
                        GameTime.TimeOfDayHours <= ToHour);
        }

        // Start switching lights on/off depending on the hour.
        if (!isSwitching && 
            ((IsInHour && !lit) || (!IsInHour && lit)))
        {
            isSwitching = true;
            hourAtSwitch = GameTime.TimeOfDayHours;
            switchDelay = Random.Range(0.0f, MaxRandomDelaySwitchingHours);
        }

        // When finished switching turn the light on/off.
        if (isSwitching &&
            GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, hourAtSwitch).shortest >= switchDelay)
        {
            isSwitching = false;
            lit = !lit;

            // Switch the dynamic lights.
            foreach (Light light in GetComponentsInChildren<Light>())
            {
                light.enabled = !light.enabled;
            }

            // Switch the material lights.
            if (LitMaterial && UnlitMaterial)
            {
                foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                {
                    if (lit && renderer.sharedMaterial == UnlitMaterial)
                    {
                        renderer.sharedMaterial = LitMaterial;
                    }
                    else if (!lit && renderer.sharedMaterial == LitMaterial)
                    {
                        renderer.sharedMaterial = UnlitMaterial;
                    }
                }
            }
        }
    }

}
