﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryItem : MonoBehaviour {

    public InventoryItemDescription InventoryItemDescription;

    public string ItemName;
    public string PrimaryActionDescription;
    [Header("Used in calculation when selling the item")]
    public float ItemValue;
    public bool CanBeSold = true;
    public int InventoryIndex;

    // Override to do some action when the primary item action is performed.
    public virtual void OnPrimaryAction() {}

    // Override to set custom hide behaviour.
    public virtual void Hide()
    {
        GetComponent<Image>().enabled = false;
    }

    // Override to set custom show behaviour.
    public virtual void Show()
    {
        GetComponent<Image>().enabled = true;
    }

    // Override to do something when the user moves the mouse over the item in the inventory.
    // Call from derived.
    public virtual void OnCursorEnter()
    {
        if (InventoryItemDescription)
        {
            InventoryItemDescription.Source = gameObject;
        }
        else
        {
            InventoryItemDescription.Source = null;
        }
    }

    // Override to do something when the user moves the mouse away from the item in the inventory.
    public virtual void OnCursorExit() {}

    // Override to do something when the player attempts to sell the item.
    public virtual void OnSellRequested() { }

}
