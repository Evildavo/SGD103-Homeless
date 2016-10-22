using UnityEngine;
using System.Collections.Generic;

public class Greg : Character {

    public Trigger Trigger;
    public PlayerCharacter PlayerCharacter;
    public AudioClip HelloAudio;
    public Menu Menu;
    public PlayerState PlayerState;

    public float DrugsCost;
    public float AlcoholCost;

    new void Start()
    {
        base.Start();
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
    }

    new void Update()
    {
        base.Update();
    }

    public void OnTrigger()
    {
        Speak("Hi, my name's Greg. Would you like to buy something?", HelloAudio, showBuyMenu);
    }

    public void OnPlayerExit()
    {
        reset();
    }

    public void OnTriggerUpdate()
    {
        // Leave menu on E key.
        if (Menu.IsDisplayed() && Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }

    void showBuyMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onBuyDrugsSelected, "Drugs", DrugsCost, PlayerState.CanAfford(DrugsCost)));
        options.Add(new Menu.Option(onBuyAlcoholSelected, "Cheap Alcohol", AlcoholCost, PlayerState.CanAfford(AlcoholCost)));
        options.Add(new Menu.Option(onExitSelected, "Exit"));

        Menu.Show(options);
    }

    void onBuyDrugsSelected()
    {
        PlayerCharacter.Speak("(No. I just can't do that)");
        showBuyMenu();
    }

    void onBuyAlcoholSelected()
    {
        Speak("Sure thing.");
        Debug.Log("Buying alcohol"); //
        showBuyMenu();
    }

    void onExitSelected()
    {
        reset();
    }

    void reset()
    {
        Menu.Hide();
        Trigger.Reset();
    }

}
