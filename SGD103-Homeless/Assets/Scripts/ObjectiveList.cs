using UnityEngine;
using System.Collections;

public class ObjectiveList : MonoBehaviour {

    public Objective ObjectivePrefab;
    public Transform BasePosition;

    public Objective NewObjective(string text)
    {
        // Push other objectives down one slot.
        foreach (Objective other in GetComponentsInChildren<Objective>())
        {
            other.SlotNum++;
        }

        // Create new objective.
        Objective objective = Instantiate(ObjectivePrefab, transform, false) as Objective;
        objective.ObjectiveList = this;
        objective.BasePosition = BasePosition;
        objective.ObjectiveName = text;        
        return objective;
    }

    public void RemoveObjective(Objective objective)
    {
        // Move other objectives below it up one slot.
        foreach (Objective other in GetComponentsInChildren<Objective>())
        {
            if (other.SlotNum > objective.SlotNum)
            {
                other.SlotNum--;
            }
        }

        // Destroy the objective.
        Destroy(objective.gameObject);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            NewObjective("Get more alcohol");
        }
    }

}
