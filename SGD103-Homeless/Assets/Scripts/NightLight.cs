using UnityEngine;
using System.Collections;

public class NightLight : MonoBehaviour {
    
    public GameTime GameTime;

    void Start () {
	
	}
	
	void Update () {
	    if (GameTime.IsNight)
        {
            GetComponent<Light>().enabled = true;
        }
        else
        {
            GetComponent<Light>().enabled = false;
        }
	}
}
