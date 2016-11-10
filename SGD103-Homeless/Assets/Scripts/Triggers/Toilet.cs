using UnityEngine;
using System.Collections.Generic;

public class Toilet : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;
    
    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnExit, "Exit"));

        Main.Menu.Show(options);
    }

    void Start ()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnCloseRequested(reset);
    }

    void OnTrigger()
    {
        OpenMainMenu();
    }

    void OnExit()
    {
        reset();
        Main.MessageBox.ShowNext();
    }
    
    void reset()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }

}
