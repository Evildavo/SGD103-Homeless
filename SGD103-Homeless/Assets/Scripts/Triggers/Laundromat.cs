using UnityEngine;
using System.Collections.Generic;

public class Laundromat : MonoBehaviour
{ 
    public Main Main;
    public Trigger Trigger;
    public WashClothesEvent WashClothesEvent;

    public float CostToUse;

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            onWashClothes, "Wash and dry clothes", CostToUse, Main.PlayerState.CanAfford(CostToUse)));
        options.Add(new Menu.Option(OnExit, "Exit"));

        Main.Menu.Show(options);
    }

    public void onWashClothes()
    {
        // Pay money.
        Main.PlayerState.Money -= CostToUse;

        WashClothesEvent.Attend();
    }

    void Start ()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnCloseRequested(reset);
    }
    
	void OnTrigger () {
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
