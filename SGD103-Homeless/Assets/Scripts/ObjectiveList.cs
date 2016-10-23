using UnityEngine;
using System.Collections;

public class ObjectiveList : MonoBehaviour {

    public Objective ObjectivePrefab;
    public Transform BasePosition;

    [Header("Objectives are removed when full")]
    public int NumSlots;

    public Objective NewObjective(string name)
    {
        // Push other objectives down one slot.
        foreach (Objective other in GetComponentsInChildren<Objective>())
        {
            other.SlotNum++;
            if (other.SlotNum > NumSlots - 1)
            {
                other.Disappearing = true;
            }
        }

        // Create new objective.
        Objective objective = Instantiate(ObjectivePrefab, transform, false) as Objective;
        objective.ObjectiveList = this;
        objective.BasePosition = BasePosition;
        objective.ObjectiveName = name;    
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

}
