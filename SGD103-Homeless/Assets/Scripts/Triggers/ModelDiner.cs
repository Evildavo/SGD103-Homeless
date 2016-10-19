using UnityEngine;
using System.Collections.Generic;

public class ModelDiner : MonoBehaviour
{
    public Trigger Trigger;
    public Menu Menu;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    public void ShowMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnExit, "Exit"));
        Menu.Show(options);
    }

    void reset()
    {
        Menu.Hide();
        if (Trigger)
        {
            Trigger.Reset();
        }
    }

    public void OnExit()
    {
        reset();
    }

    public void OnTrigger()
    {
        ShowMainMenu();
    }

    public void OnPlayerExit()
    {
        reset();
    }

    public void OnTriggerUpdate()
    {
        // Leave menu on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }
}
