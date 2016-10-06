using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuTriggerTest : Trigger {

    public Menu Menu;
    
    public override void OnTrigger()
    {
        Menu.Option[] options = {
            new Menu.Option("Buy food", 10),
            new Menu.Option("Buy alcohol", 20),
            new Menu.Option("Say A"),
            new Menu.Option("Say B")
        };
        Menu.SetOptions(options);
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
