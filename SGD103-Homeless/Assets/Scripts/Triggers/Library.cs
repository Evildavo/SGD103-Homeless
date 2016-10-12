using UnityEngine;
using System.Collections.Generic;

public class Library : MonoBehaviour {
    private Trigger trigger;

    public Menu Menu;
    public List<Menu.Option> Options;

    public void OnTrigger(Trigger trigger)
    {
        this.trigger = trigger;
        Menu.Show(Options);
    }

    public void OnJobSearch(string name, int value)
    {
        Debug.Log("Searching for job");
    }

    public void OnReadBooks(string name, int value)
    {
        Debug.Log("Reading books");
    }

    public void OnExit(string name, int value)
    {
        Menu.Hide();
        trigger.Reset();
    }

    public void OnPlayerExit(Trigger trigger)
    {
        Menu.Hide();
        trigger.Reset();
    }
    
}
