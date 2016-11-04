using UnityEngine;
using System.Collections;

public class WashClothesEvent : EventAtLocation
{
    public bool IncludeClothesPlayerIsWearing = true;

    public void WashClothes()
    {
        // Wash what the player is wearing.
        if (IncludeClothesPlayerIsWearing && Main.PlayerState.CurrentClothing)
        {
            Main.PlayerState.CurrentClothing.Cleanliness = 1.0f;

            // Fulfill cleanliness objective.
            Main.PlayerState.CompleteCleanlinessObjectives();
        }

        // Wash all clothes in the player inventory.
        bool hasClothesInInventory = false;
        foreach (ClothingItem item in Main.Inventory.ItemContainer.GetComponentsInChildren<ClothingItem>())
        {
            item.Cleanliness = 1.0f;
            hasClothesInInventory = true;
        }

        // Open the inventory to show the clean clothes.
        if (hasClothesInInventory)
        {
            Main.Inventory.ShowPreview();
        }
    }

    protected override void OnPlayerAttends()
    {
    }

    protected override void OnPlayerLeaves()
    {
        WashClothes();

        Main.UI.DisableModalMode();
                
        // Show message that clothes were cleaned.
        Main.MessageBox.ShowForTime("Clothes washed", null, gameObject);
    }

    new void Update () {
        base.Update();
	}
}
