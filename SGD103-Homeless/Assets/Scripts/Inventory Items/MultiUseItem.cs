using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiUseItem : InventoryItem {
    
    public int NumUses = 1;
    public string PluralName;
    
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


}
