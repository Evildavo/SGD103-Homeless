using UnityEngine;
using System.Collections.Generic;

public class Greg : MonoBehaviour {

    public CharacterTrigger CharacterTrigger;
    public AudioClip HelloAudio;
    public Menu Menu;

    void Start()
    {
        CharacterTrigger.RegisterOnTriggerListener(OnTrigger);
        CharacterTrigger.RegisterOnPlayerExitListener(OnPlayerExit);
        CharacterTrigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
    }

    public void OnTrigger()
    {
        CharacterTrigger.Speak("Hi, my name's Greg. Would you like to buy something?", HelloAudio, showBuyMenu);
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
        options.Add(new Menu.Option(onExitSelected, "Exit"));

        Menu.Show(options);
    }

    void onExitSelected()
    {
        reset();
    }

    void reset()
    {
        Menu.Hide();
        CharacterTrigger.Reset();
    }

}
