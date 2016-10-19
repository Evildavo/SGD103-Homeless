﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory : MonoBehaviour
{
    private float timeAtPreviewStart;
    private bool isHidden = false;

    public InventoryItemDescription ItemDescription;
    public Transform SlotContainer;
    public Transform ItemContainer;
    public InventoryHudButton InventoryHudButton;

    [Tooltip("A value of 0 will disable hiding")]
    public int DeadZonePixels = 25;
    public bool CloseOnItemUse = true;
    public bool CloseOnClickOutside = true;
    public bool HiddenAtStart = true;
    public float PreviewTime = 2.0f;
    public bool OpenPreviewOnItemAdded = true;
    [ReadOnly]
    public bool IsPreviewing = false;
    [ReadOnly]
    public bool IsInventoryFull = false;

    // Shows the inventory.
    public void Show()
    {
        IsPreviewing = false;
        isHidden = false;
        ItemDescription.ItemName.text = "";
        ItemDescription.ItemAction.text = "";
        ItemDescription.GetComponent<Image>().enabled = false;
        ItemDescription.gameObject.SetActive(true);
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            slot.Show();
        }
    }

    // Temporarily shows the inventory before closing automatically.
    // Doesn't work if the inventory was already open (not on a preview).
    public void ShowPreview()
    {
        timeAtPreviewStart = Time.time;
        if (isHidden)
        {
            Show();
            IsPreviewing = true;
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
        if (item)
        {
            foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
            {
                if (slot.Item.gameObject == item.gameObject)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    // Instantiate the item before calling if it's a prefab.
    public void AddItem(InventoryItem item)
    {
        if (IsInventoryFull)
        {
            Debug.LogWarning("Inventory is full.");
            return;
        }

        // Open inventory preview.
        if (OpenPreviewOnItemAdded)
        {
            ShowPreview();
        }

        // Add to next available free slot.
        int i = 0;
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.Item)
            {
                item.InventoryIndex = i;
                item.transform.position = slot.transform.position;
                item.transform.SetParent(ItemContainer, true);
                item.transform.localScale = slot.transform.localScale;
                slot.Item = item;
                if (isHidden)
                {
                    slot.Hide();
                }
                return;
            }
            i++;
        }
    }
    
    // Removes the item and moves other items to fill the gaps.
    // Warning: Currently also destroys the object.
    public void RemoveItem(InventoryItem item)
    {
        // Move items down to fill the slot.
        InventorySlot[] inventorySlots = SlotContainer.GetComponentsInChildren<InventorySlot>(true);
        for (var i = item.InventoryIndex; i < inventorySlots.Length; i++)
        {
            if (i == inventorySlots.Length - 1)
            {
                inventorySlots[i].Item = null;
            }
            else
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
        // Check if inventory is full.
        IsInventoryFull = true;
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.Item)
            {
                IsInventoryFull = false;
            }
        }

        // Handle previewing.
        if (IsPreviewing)
        {
            // Keep preview open if the cursor is over an item.
            if (ItemDescription.gameObject.activeInHierarchy && ItemDescription.ItemName.text != "")
            {
                timeAtPreviewStart = Time.time;
            }

            // Close the inventory after preview time has passed.
            if (Time.time - timeAtPreviewStart > PreviewTime)
            {
                IsPreviewing = false;
                Hide();
            }
        }

        // Close if the mouse was clicked while not over an item.
        if (CloseOnClickOutside && Input.GetButtonDown("Primary") &&
            (!ItemDescription.gameObject.activeInHierarchy || ItemDescription.ItemName.text == "") &&
            !InventoryHudButton.IsCursorOver)
        {
            Hide();
        }
    }

}
