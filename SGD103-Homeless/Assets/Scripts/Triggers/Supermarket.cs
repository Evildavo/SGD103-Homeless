using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Supermarket : MonoBehaviour {

    private List<Menu.Option> mainMenuOptions = new List<Menu.Option>();

    public Trigger Trigger;
    public Menu Menu;


    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        
        mainMenuOptions.Add(new Menu.Option(OnFoodMenuSelected, "Buy food"));
        mainMenuOptions.Add(new Menu.Option(OnExitSelected, "Exit"));
    }

    public void OnFoodMenuSelected()
    {
        List<Menu.Option> subMenuOptions = new List<Menu.Option>();
        subMenuOptions.Add(new Menu.Option(OnBackButtonSelected, "Back"));
        Menu.Show(subMenuOptions);
    }

    public void OnBackButtonSelected()
    {
        Menu.Show(mainMenuOptions);
    }

    public void OnExitSelected()
    {
        reset();
    }


    public void OnTrigger()
    {
        Menu.Show(mainMenuOptions);
        Debug.Log("Yeah Boi");
    }

    public void OnPlayerExit()
    {
        Menu.Hide();
        reset();
    }

    void reset()
    {
        Menu.Hide();
        if (Trigger)
        {
            Trigger.Reset();
        }
    }

    public void OnTriggerUpdate()
    {
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }


}
