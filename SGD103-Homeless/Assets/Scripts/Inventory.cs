using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {

    public Transform InventorySlotContainer;

    public InventorySlot[] GetSlots()
    {
        return InventorySlotContainer.GetComponentsInChildren<InventorySlot>();
    }
    
	void Start () {
	    
	}
	
	void Update () {
	
	}
}
