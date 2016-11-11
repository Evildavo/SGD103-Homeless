using UnityEngine;
using System.Collections.Generic;

public class Laundromat : MonoBehaviour
{ 
    public Main Main;
    public Trigger Trigger;
    public WashClothesEvent WashClothesEvent;
    public AudioClip Ambience;
    public Sprite Splash;

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

        // Show splash screen.
        Main.Splash.Show(Splash);

        // Play ambience audio.
        var audio = GetComponent<AudioSource>();
        if (audio.clip != Ambience)
        {
            audio.clip = Ambience;
            audio.time = 0.0f;
            audio.loop = true;
            audio.Play();
        }
    }

    void OnExit()
    {
        reset();
        Main.MessageBox.ShowNext();
    }

    void reset()
    {
        // Hide splash screen.
        Main.Splash.Hide();

        // Stop the ambience.
        var audio = GetComponent<AudioSource>();
        audio.Stop();
        audio.clip = null;

        Main.Menu.Hide();
        Trigger.Reset();
    }

}
