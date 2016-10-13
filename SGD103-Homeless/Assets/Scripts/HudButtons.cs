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
        // Note: If CloseOnClickOutside is enabled the inventory will close itself.
        bool inventoryHidden = !Inventory.gameObject.activeSelf;
        if (inventoryHidden)
        {
            Inventory.gameObject.SetActive(true);
        }
        else if (!Inventory.CloseOnClickOutside)
        {
            Inventory.gameObject.SetActive(false);
        }
    }

}
