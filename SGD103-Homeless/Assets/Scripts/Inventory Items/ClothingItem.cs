using UnityEngine;
using System.Collections;

public class ClothingItem : InventoryItem
{    

    // Call from derived.
    public override void OnPrimaryAction()
    {
        Debug.Log("Changed to clothing");
    }

}
