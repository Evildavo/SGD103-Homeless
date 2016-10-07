using UnityEngine;
using System.Collections;

public class NightLight : MonoBehaviour {
    private bool switched = true;
    
    public GameTime GameTime;

    public float MaxSwitchDelaySeconds;
    public bool IsVehicleLight = false;

    void Start ()
    {
        if (GameTime.IsNight)
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
            if (GameTime.IsNight && GetComponent<Light>().enabled == false)
            {
                switched = false;
                Invoke("EnableLight", Random.Range(0.0f, MaxSwitchDelaySeconds));
            }
            else if (!GameTime.IsNight && GetComponent<Light>().enabled == true)
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
