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
        Main.UI.ReturnTo = OpenMainMenu;

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
        audio.time = 1.5f;
        audio.loop = true;
        audio.Play();

        // Stop street audio.
        if (Main.Ambience)
        {
            Main.Ambience.Pause();
        }

        // Allow player to use their inventory.
        Main.PlayerState.IsInPrivate = true;
        OpenMainMenu();

        Main.Inventory.Show();
    }

    void OnExit()
    {
        reset();
        Main.MessageBox.ShowNext();
    }
    
    void reset()
    {
        Main.PlayerState.IsInPrivate = false;
        Main.UI.ReturnTo = null;

        // Hide splash screen.
        Main.Splash.Hide();

        // Play toilet flush sound.
        var audio = GetComponent<AudioSource>();
        audio.clip = FlushSound;
        audio.time = 0.0f;
        audio.loop = false;
        audio.Play();

        // Resume street audio.
        if (Main.Ambience)
        {
            Main.Ambience.Resume();
        }

        Main.Inventory.Hide();
        Main.Menu.Hide();
        Trigger.Reset();
    }

}
