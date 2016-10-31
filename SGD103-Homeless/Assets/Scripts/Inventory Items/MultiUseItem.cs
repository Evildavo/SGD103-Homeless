using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiUseItem : InventoryItem {
    
    [Header("Note: Item Value is multiplied by NumUses")]
    public int NumUses = 1;
    public string PluralName;

    public override float GetItemValue()
    {
        return ItemValue * NumUses;
    }

    // Call on update in derived.
    public void UpdateItemDescription()
    {
        // Show plural name if there's more than one item.
        if (Main.ItemDescription.Source == gameObject)
        {
            if (NumUses == 1)
            {
                Main.ItemDescription.ItemName.text = ItemName;
            }
            else
            {
                Main.ItemDescription.ItemName.text =
                    NumUses.ToString() + " " + PluralName;
            }
        }
    }
    
    // Call from derived.
    protected void Start()
    {
        if (PluralName == "")
        {
            Debug.LogError("You forgot to give the item \"" + ItemName + "\" a plural name. Defaulting to item name.");
            PluralName = ItemName;
        }
    }

}
