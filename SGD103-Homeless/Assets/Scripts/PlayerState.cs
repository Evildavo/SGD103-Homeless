using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float Hunger = 0;

    public float HungerIncreasePerSecond;
    
	void Start () {
	
	}
	
	void Update () {
        Hunger += HungerIncreasePerSecond * Time.deltaTime;
        if (Hunger > 1.0f)
        {
            Hunger = 1.0f;
        }
    }
}
