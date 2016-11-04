using UnityEngine;
using System.Collections;

public class ClothingItem : InventoryItem
{
    public float Cleanliness = 1.0f;
    
    // Call from derived.
    public override void OnPrimaryAction()
    {
        // Swap the selected clothes with what the player was wearing.
        if (Main.PlayerState.CurrentClothing)
        {
            Main.Inventory.AddItem(Main.PlayerState.CurrentClothing);
        }
        Main.Inventory.RemoveItem(this, false);
        transform.SetParent(Main.PlayerState.transform);
        Main.PlayerState.CurrentClothing = this;

        // Update cleanliness.
        //...
    }

    void Update()
    {
        // Clothes get dirtier over time while worn.
        if (Main.PlayerState.CurrentClothing == this)
        {
            if (Cleanliness > 0.0f)
            {
                Cleanliness -= Main.PlayerState.ClothesDirtiedPerHourWorn * Main.GameTime.GameTimeDelta;
            }
            else
            {
                Cleanliness = 0.0f;
            }
        }
    }

}
