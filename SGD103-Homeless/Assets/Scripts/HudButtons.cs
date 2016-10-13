using UnityEngine;
using System.Collections;

public class HudButtons : MonoBehaviour {

    public Inventory Inventory;
    
	void Start () {
	
	}
	
	void Update () {

    }

    public void OnWatchClick()
    {

    }

    public void OnSleepClick()
    {

    }

    public void OnInventoryClick()
    {
        // Toggle inventory.
        bool showInventory = !Inventory.gameObject.activeSelf;
        Inventory.gameObject.SetActive(showInventory);
    }

}
