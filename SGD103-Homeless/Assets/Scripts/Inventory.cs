using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour
{
    private bool isAwake = false;
    private float timeAtWake;
    private Vector3 lastMousePosition;
    
    public Transform ItemDescription;
    public Transform SlotContainer;
   
    [Tooltip("A value of 0 will disable hiding")]
    public float HideAfterSeconds = 1.0f;
    public int DeadZonePixels = 25;
    public bool CloseOnItemUse = true;
    public bool CloseOnClickOutside = true;

    // Returns true if the inventory is full.
    public bool IsInventoryFull()
    {
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.GetItem())
            {
                return false;
            }
        }
        return true;
    }
    
    // Instantiate the item before calling.
    public void AddItem(InventoryItem item)
    {
        // Add to next available free slot.
        int i = 0;
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.GetItem())
            {
                WakeInventory();
                item.InventoryIndex = i;
                item.transform.SetParent(slot.transform, false);
                return;
            }
            i++;
        }
        Debug.Log("Warning: Inventory is full.");
    }

    // Removes the item and moves other items to fill the gaps.
    public void RemoveItem(InventoryItem item)
    {
        // Move items down to fill the slot.
        InventorySlot[] inventorySlots = SlotContainer.GetComponentsInChildren<InventorySlot>(true);
        for (var i = item.InventoryIndex; i < inventorySlots.Length - 1; i++)
        {
            InventoryItem itemAbove = inventorySlots[i + 1].GetItem();
            if (itemAbove)
            {
                itemAbove.InventoryIndex = i;
                itemAbove.transform.SetParent(inventorySlots[i].transform, false);
            }
        }
        
        // Destroy the object.
        Destroy(item.gameObject);
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

    void Start()
    {
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Close if the mouse was clicked while not over an item.
        if (CloseOnClickOutside && Input.GetButtonUp("Primary") &&
            !ItemDescription.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

        // Wake up the inventory if the mouse moved or is over an item.
        if (ItemDescription.gameObject.activeInHierarchy ||
            Mathf.Abs(lastMousePosition.x - Input.mousePosition.x) > DeadZonePixels ||
            Mathf.Abs(lastMousePosition.y - Input.mousePosition.y) > DeadZonePixels)
        {
            WakeInventory();
        }

        // After time hide the inventory.
        if (isAwake && HideAfterSeconds != 0 && Time.time - timeAtWake > HideAfterSeconds)
        {
            isAwake = false;
            foreach (Transform slot in SlotContainer.transform)
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

}
