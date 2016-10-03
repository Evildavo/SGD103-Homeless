using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour
{
    private bool isAwake = true;
    private float timeAtWake;
    private Vector3 lastMousePosition;
    private int lastMouseY;

    public Transform ItemDescription;
    public Transform SlotContainer;
   
    public float HideAfterSeconds = 1.0f;
    public int DeadZonePixels = 25;
    
    // Instantiate the item before calling.
    public void AddItem(InventoryItem item)
    {
        // Add to next available free slot.
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.GetItem())
            {
                WakeInventory();
                item.transform.SetParent(slot.transform, false);
                return;
            }
        }
    }

    // Displays the inventory.
    public void WakeInventory()
    {
        isAwake = true;
        timeAtWake = Time.time;
        foreach (Transform slot in SlotContainer.transform)
        {
            slot.gameObject.SetActive(true);
        }
        lastMousePosition = Input.mousePosition;
    }
    
    void Update()
    {
        // Wake up the inventory if the mouse moved or is over an item.
        if (ItemDescription.gameObject.activeInHierarchy ||
            Mathf.Abs(lastMousePosition.x - Input.mousePosition.x) > DeadZonePixels ||
            Mathf.Abs(lastMousePosition.y - Input.mousePosition.y) > DeadZonePixels)
        {
            WakeInventory();
        }

        // After time hide the inventory.
        if (isAwake && Time.time - timeAtWake > HideAfterSeconds)
        {
            isAwake = false;
            foreach (Transform slot in SlotContainer.transform)
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

}
