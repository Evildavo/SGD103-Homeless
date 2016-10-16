﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryItem : MonoBehaviour {

    public string ItemName;
    public string PrimaryActionDescription;
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
    public virtual void OnCursorEnter() {}

    // Override to do something when the user moves the mouse away from the item in the inventory.
    public virtual void OnCursorExit() {}

}
