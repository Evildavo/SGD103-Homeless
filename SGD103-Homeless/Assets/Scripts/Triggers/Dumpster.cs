using UnityEngine;
using System.Collections;

public class Dumpster : Trigger {
    private float timeAtLastCheck;

    public PlayerState PlayerState;

    public bool Searching = false;

    public override void OnTrigger()
    {
        Searching = true;
        timeAtLastCheck = Time.time;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

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

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        Searching = false;
        IsActive = true;
    }
}
