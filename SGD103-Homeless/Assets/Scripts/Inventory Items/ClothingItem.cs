using UnityEngine;
using System.Collections;

public class ClothingItem : InventoryItem
{    

    // Call from derived.
    public override void OnPrimaryAction()
    {
        // Swap the selected clothes with what the player was wearing.
        Main.Inventory.AddItem(Main.PlayerState.CurrentClothing);
        Main.Inventory.RemoveItem(this, false);
        transform.SetParent(Main.PlayerState.transform);
        Main.PlayerState.CurrentClothing = this;

        // Update cleanliness.
        //...
    }

}
