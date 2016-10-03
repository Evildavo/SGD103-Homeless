using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {
    private bool isAwake = true;
    private float timeAtWake;

    public Transform InventorySlotContainer;

    public float HideAfterSeconds = 4.0f;

    public InventorySlot[] GetSlots()
    {
        return InventorySlotContainer.GetComponentsInChildren<InventorySlot>();
    }
    
	void Start () {
	    
	}

    void Update()
    {
        // If mouse moved wake up inventory.
        if (Input.GetAxis("Mouse X") != 0 && Input.GetAxis("Mouse Y") != 0)
        {
            isAwake = true;
            timeAtWake = Time.time;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        // After time hide the inventory.
        if (isAwake && Time.time - timeAtWake > HideAfterSeconds)
        {
            isAwake = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

}
