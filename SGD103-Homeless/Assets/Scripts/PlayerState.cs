using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour {

    public GameTime GameTime;

    [Range(0.0f, 1.0f)]
    public float Hunger = 0;
    public float HungerIncreasePerGameHour;
    
	void Start () {
	
	}
	
	void Update () {
        Hunger += HungerIncreasePerGameHour / 60.0f / 60.0f * Time.deltaTime * GameTime.TimeScale;
        if (Hunger > 1.0f)
        {
            Hunger = 1.0f;
        }
    }
}
