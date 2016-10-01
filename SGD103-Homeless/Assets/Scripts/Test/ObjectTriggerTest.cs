using UnityEngine;
using System.Collections;

public class ObjectTriggerTest : Trigger {

    public PlayerState PlayerState;

    public override void OnTrigger()
    {
        PlayerState.Money = 0.0f;
        PlayerState.HungerThirst = 1.0f;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }
}
