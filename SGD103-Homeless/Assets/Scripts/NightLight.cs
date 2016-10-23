using UnityEngine;
using System.Collections;

public class NightLight : MonoBehaviour {
    private bool switched = true;

    public enum LightType
    {
        GENERIC,
        STREET,
        VEHICLE,
        SKY
    }

    public Main Main;

    public float MaxSwitchDelaySeconds;
    public LightType Type = LightType.GENERIC;

    void Start ()
    {
        if (Main.GameTime.IsNight)
        {
            GetComponent<Light>().enabled = true;
        }
        else
        {
            GetComponent<Light>().enabled = false;
        }
    }
	
	void Update () {
        if (switched)
        {
            if (Main.GameTime.IsNight && GetComponent<Light>().enabled == false)
            {
                switched = false;
                Invoke("EnableLight", Random.Range(0.0f, MaxSwitchDelaySeconds));
            }
            else if (!Main.GameTime.IsNight && GetComponent<Light>().enabled == true)
            {
                switched = false;
                Invoke("DisableLight", Random.Range(0.0f, MaxSwitchDelaySeconds));
            }
        }
	}
    
    void EnableLight() {
        GetComponent<Light>().enabled = true;
        switched = true;
    }

    void DisableLight() {
        GetComponent<Light>().enabled = false;
        switched = true;
    }
}
