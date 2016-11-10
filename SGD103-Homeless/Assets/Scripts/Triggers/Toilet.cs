using UnityEngine;
using System.Collections.Generic;

public class Toilet : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;
    public AudioClip FlushSound;

    // Splash screen and ambient sound.
    public AudioClip ToiletAmbience;
    public Sprite ToiletSplash;
    
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
        // Show splash screen.
        Main.Splash.Show(ToiletSplash);

        // Play ambience audio.
        var audio = GetComponent<AudioSource>();
        audio.clip = ToiletAmbience;
        audio.time = 0.0f;
        audio.loop = true;
        audio.Play();

        // Allow player to use their inventory.
        Main.PlayerState.IsInPrivate = true;
        OpenMainMenu();

        Main.Inventory.Show();
    }

    void OnExit()
    {
        reset();
        Main.MessageBox.ShowNext();

        // Play toilet flush sound.
        var audio = GetComponent<AudioSource>();
        audio.clip = FlushSound;
        audio.time = 0.0f;
        audio.loop = false;
        audio.Play();
    }
    
    void reset()
    {
        // Hide splash screen.
        Main.Splash.Hide();
        
        Main.Inventory.Hide();
        Main.Menu.Hide();
        Trigger.Reset();
        Main.PlayerState.IsInPrivate = false;
    }

}
