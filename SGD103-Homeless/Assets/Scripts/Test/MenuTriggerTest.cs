using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuTriggerTest : Trigger {
    
    public override void OnTrigger()
    {
        IsActive = true;
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
}
