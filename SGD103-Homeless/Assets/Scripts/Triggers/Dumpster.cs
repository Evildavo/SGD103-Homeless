using UnityEngine;
using System.Collections;

public class Dumpster : MonoBehaviour {
    private float timeAtLastCheck;

    public Trigger Trigger;
    public PlayerState PlayerState;

    public bool Searching = false;

    public void OnTrigger()
    {
        Searching = true;
        timeAtLastCheck = Time.time;
    }
    
    void Update()
    {
        if (Time.time - timeAtLastCheck > 1)
        {
            float value = Random.Range(0, 100);
            if (value < 10)
            {
                PlayerState.HungerThirst += 0.2f;
                PlayerState.HungerThirst -= 0.2f;
            }
        }
    }
    
    public void OnPlayerExit()
    {
        Searching = false;
        Trigger.IsActive = true;
    }
}
