using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private bool isReading;

    public Trigger Trigger;
    public Menu Menu;
    public MessageBox MessageBox;
    public PlayerState PlayerState;
    public List<Menu.Option> MainMenu;
    public List<Menu.Option> ReadingMenu;

    public float MoraleGainedPerSecond = 0.05f;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    public void OnTrigger()
    {
        Menu.Show(MainMenu);
    }
    
    public void OnPlayerExit()
    {
        leaveTrigger();
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
            leaveTrigger();
        }
    }

    public void OnJobSearch(string name, int value)
    {
        Debug.Log("Searching for job");
    }

    public void OnReadBooks(string name, int value)
    {
        MessageBox.SetMessage("You are reading...");
        MessageBox.Show(gameObject);
        Menu.Show(ReadingMenu);
        PlayerState.HighlightMorale = true;
        Trigger.GameTime.IsTimeAccelerated = true;
        isReading = true;
    }

    public void OnExit(string name, int value)
    {
        Menu.Hide();
        Trigger.Reset();
    }

    public void OnStopReading(string name, int value)
    {
        Menu.Show(MainMenu);
        MessageBox.Hide();
        PlayerState.HighlightMorale = false;
        Trigger.GameTime.IsTimeAccelerated = false;
        isReading = false;
    }

    void leaveTrigger()
    {
        Menu.Hide();
        if (Trigger)
        {
            Trigger.Reset();
        }
        MessageBox.Hide();
        isReading = false;
        PlayerState.HighlightMorale = false;
    }
    
}
