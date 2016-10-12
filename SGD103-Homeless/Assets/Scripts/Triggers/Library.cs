using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private bool isReading;

    public Trigger Trigger;
    public Menu Menu;
    public MessageBox MessageBox;
    public PlayerState PlayerState;

    public float MoraleGainedPerSecond = 0.05f;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    // Reset to starting values.
    void reset()
    {
        Menu.Hide();
        MessageBox.Hide();
        if (Trigger)
        {
            Trigger.Reset();
        }
    }

    // Creates and returns a new main menu.
    List<Menu.Option> getMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnJobSearch, "Job search"));
        options.Add(new Menu.Option(OnReadBook, "Read book"));
        options.Add(new Menu.Option(OnExit, "Exit"));
        return options;
    }

    // Creates and returns a new reading menu.
    List<Menu.Option> getReadingMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnStopReading, "Stop reading"));
        return options;
    }

    public void OnJobSearch()
    {
        MessageBox.Show("Searching for jobs...", gameObject);
    }

    public void OnReadBook()
    {
        MessageBox.Show("You are reading...", gameObject);
        Menu.Show(getReadingMenu());
        PlayerState.HighlightMorale = true;
        Trigger.GameTime.IsTimeAccelerated = true;
        isReading = true;
    }

    public void OnExit()
    {
        reset();
    }

    public void OnStopReading()
    {
        Menu.Show(getMainMenu());
        MessageBox.Hide();
        PlayerState.HighlightMorale = false;
        Trigger.GameTime.IsTimeAccelerated = false;
        isReading = false;
    }

    public void OnTrigger()
    {
        Menu.Show(getMainMenu());
    }

    public void OnPlayerExit()
    {
        reset();
    }

    public void OnTriggerUpdate()
    {
        // Increase morale while reading.
        if (isReading)
        {
            PlayerState.Morale += MoraleGainedPerSecond * Time.deltaTime;
        }

        // Leave menu on E key.
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }

}
