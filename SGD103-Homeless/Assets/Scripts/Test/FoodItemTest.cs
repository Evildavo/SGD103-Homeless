using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FoodItemTest : InventoryItem
{
    
    public float CloseAfterSeconds = 1.75f;

    public override void OnPrimaryAction()
    {
        Main.MessageBox.ShowForTime("You feel full", CloseAfterSeconds, gameObject);
        Main.PlayerState.ChangeNutrition(1.0f);

        Main.Inventory.RemoveItem(this);
    }

}
