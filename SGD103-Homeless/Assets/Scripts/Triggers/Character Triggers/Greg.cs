using UnityEngine;
using System.Collections.Generic;

public class Greg : MonoBehaviour {

    public CharacterTrigger CharacterTrigger;
    public AudioClip HelloAudio;
    public Menu Menu;

    public void OnTrigger()
    {
        CharacterTrigger.Speak("Hi, my name's Greg. Would you like to buy something?", HelloAudio, showBuyMenu);
    }

    public void OnPlayerExit()
    {
        reset();
    }
    
    void Start()
    {
        CharacterTrigger.RegisterOnTriggerListener(OnTrigger);
        CharacterTrigger.RegisterOnPlayerExitListener(OnPlayerExit);
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
