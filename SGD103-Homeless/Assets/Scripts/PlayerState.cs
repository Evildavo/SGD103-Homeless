using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour {

    public GameTime GameTime;

    [Range(0.0f, 1.0f)]
    public float Hunger = 0;
    public float HungerIncreasePerSecond;
    
	void Start () {
	
	}
	
	void Update () {
        Hunger += HungerIncreasePerSecond * Time.deltaTime * GameTime.TimeScale;
        if (Hunger > 1.0f)
        {
            Hunger = 1.0f;
        }
    }
}
