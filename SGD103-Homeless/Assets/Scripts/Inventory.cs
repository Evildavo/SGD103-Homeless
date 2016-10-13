using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour
{
    private bool isHidden = false;

    public InventoryItemDescription ItemDescription;
    public Transform SlotContainer;
   
    [Tooltip("A value of 0 will disable hiding")]
    public int DeadZonePixels = 25;
    public bool CloseOnItemUse = true;
    public bool CloseOnClickOutside = true;
    public bool HiddenAtStart = true;

    // Shows the inventory.
    public void Show()
    {
        isHidden = false;
        ItemDescription.gameObject.SetActive(true);
        ItemDescription.ItemName.text = "";
        ItemDescription.ItemAction.text = "";
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            slot.Show();
        }
    }

    // Hides the inventory.
    public void Hide()
    {
        isHidden = true;
        ItemDescription.gameObject.SetActive(false);
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            slot.Hide();
        }
    }

    // Returns true if the inventory is hidden.
    public bool IsHidden()
    {
        return isHidden;
    }

    // Warning: Untested !!!
    // Returns true if the inventory has the given item.
    public bool HasItem(InventoryItem item)
    {
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (slot.Item.gameObject == item.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    // Returns true if the inventory is full.
    public bool IsInventoryFull()
    {
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.Item)
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
            if (!slot.Item)
            {
                item.InventoryIndex = i;
                item.transform.position = slot.transform.position;
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
            InventoryItem itemAbove = inventorySlots[i + 1].Item;
            if (itemAbove)
            {
                itemAbove.InventoryIndex = i;
                itemAbove.transform.position = inventorySlots[i].transform.position;
                inventorySlots[i].Item = itemAbove;
            }
            else
            {
                inventorySlots[i].Item = null;
            }
        }
        
        // Destroy the object.
        Destroy(item.gameObject);
    }
    
    void Start()
    {
        if (HiddenAtStart)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    
    void Update()
    {
        // Close if the mouse was clicked while not over an item.
        if (CloseOnClickOutside && Input.GetButtonUp("Primary") &&
            !ItemDescription.gameObject.activeInHierarchy)
        {
            Hide();
        }
    }

}
