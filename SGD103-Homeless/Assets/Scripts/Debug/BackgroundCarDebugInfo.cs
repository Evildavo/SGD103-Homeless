using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BackgroundCarDebugInfo : MonoBehaviour
{
#if UNITY_EDITOR
    public Material PeakHoursMaterial;
    public Material RegularHoursMaterial;
    public Material MostHoursMaterial;
    public Material AllHoursMaterial;
    
    void Update ()
    {
        // Set material colour based on active hours.
        BackgroundCar backgroundCar = GetComponent<BackgroundCar>();
        Renderer renderer = GetComponent<Renderer>();
        switch (backgroundCar.ActivePeriod)
        {
            case BackgroundCar.ActivePeriods.PEAK_HOURS:
                renderer.material = PeakHoursMaterial;
                break;
            case BackgroundCar.ActivePeriods.REGULAR_HOURS:
                renderer.material = RegularHoursMaterial;
                break;
            case BackgroundCar.ActivePeriods.MOST_HOURS:
                renderer.material = MostHoursMaterial;
                break;
            case BackgroundCar.ActivePeriods.ALL_HOURS:
                renderer.material = AllHoursMaterial;
                break;
        }
    }
#endif

}
