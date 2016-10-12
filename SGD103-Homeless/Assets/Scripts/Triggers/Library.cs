using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private Trigger trigger;
    private bool isReading;

    public Menu Menu;
    public MessageBox MessageBox;
    public PlayerState PlayerState;
    public List<Menu.Option> MainMenu;
    public List<Menu.Option> ReadingMenu;

    public float MoraleGainedPerSecond = 0.05f;

    public void OnTrigger(Trigger trigger)
    {
        this.trigger = trigger;
        Menu.Show(MainMenu);
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
        trigger.GameTime.IsTimeAccelerated = true;
        isReading = true;
    }

    public void OnExit(string name, int value)
    {
        Menu.Hide();
        trigger.Reset();
    }

    public void OnStopReading(string name, int value)
    {
        Menu.Show(MainMenu);
        MessageBox.Hide();
        PlayerState.HighlightMorale = false;
        trigger.GameTime.IsTimeAccelerated = false;
        isReading = false;
    }

    void leaveTrigger()
    {
        Menu.Hide();
        if (trigger)
        {
            trigger.Reset();
        }
        MessageBox.Hide();
        isReading = false;
        PlayerState.HighlightMorale = false;
    }


    public void OnPlayerExit(Trigger trigger)
    {
        leaveTrigger();
    }

    public void OnTriggerUpdate(Trigger trigger)
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
    
}
