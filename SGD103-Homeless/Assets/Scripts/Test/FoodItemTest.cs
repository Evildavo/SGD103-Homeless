using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FoodItemTest : InventoryItem
{
    
    public override void OnPrimaryAction()
    {
        Main.MessageBox.ShowForTime("You feel full", null, gameObject);
        Main.PlayerState.ChangeNutrition(1.0f);

        Main.Inventory.RemoveItem(this);
    }

}
