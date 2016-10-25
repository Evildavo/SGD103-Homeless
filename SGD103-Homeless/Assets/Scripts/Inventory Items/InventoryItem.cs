using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
    public Main Main;

    public string ItemName;
    public string PrimaryActionDescription;
    [Header("Used in calculation when selling the item")]
    public float ItemValue;
    public bool CanBeSold = true;
    public bool CanBeDiscarded = true;
    [Header("If adding the item to inventory in editor manually ", order=0)]
    [Space(-10, order=1)]
    [Header("set the correct index (zero indexed). Also, don't forget to", order=2)]
    [Space(-10, order = 3)]
    [Header("set the Item reference in the Inventory Slot", order = 4)]
    public int InventoryIndex;

    // Override to give a custom item value calculation.
    public virtual float GetItemValue()
    {
        return ItemValue;
    }

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
        if (Main.ItemDescription)
        {
            Main.ItemDescription.Source = gameObject;
        }
        else
        {
            Main.ItemDescription.Source = null;
        }
    }

    // Override to do something when the user moves the mouse away from the item in the inventory.
    public virtual void OnCursorExit() {}

    // Override to do something when the player attempts to sell the item.
    public virtual void OnSellRequested() { }

    // Override to do something when the player discards the item.
    public virtual void OnDiscard() { }

}
