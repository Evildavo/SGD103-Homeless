using UnityEngine;
using System.Collections;

public class SoupKitchenEvent : WeeklyEvent {

    override public void Attend()
    {
        Debug.Log("Attending soup kitchen");
    }

    new void Start () {
        base.Start();
	}
	
	new void Update () {
        base.Update();
	}
}
