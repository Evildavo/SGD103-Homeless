using UnityEngine;
using System.Collections.Generic;

public class ObjectiveTestTrigger : MonoBehaviour {
    private List<Objective> alcoholObjectives = new List<Objective>();

    public Main Main;
    public Trigger Trigger;

    void Start () {
        Trigger.RegisterOnTriggerListener(onTrigger);
        Main.Inventory.RegisterOnItemAdded(onItemAdded);
    }

    void onItemAdded(InventoryItem item)
    {
        // Remove an objective if we found alcohol.
        if (item.ItemName == "Alcohol")
        {
            if (alcoholObjectives.Count > 0)
            {
                int last = alcoholObjectives.Count - 1;
                alcoholObjectives[last].Achieved = true;
                alcoholObjectives.RemoveAt(last);
            }
        }
    }

    void onTrigger()
    {
        alcoholObjectives.Add(Main.ObjectiveList.NewObjective("Get more alcohol"));
        Trigger.Reset();
    }
        	
}
