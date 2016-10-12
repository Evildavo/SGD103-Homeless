using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FoodItemTest : InventoryItem
{
    public PlayerState PlayerState;
    public MessageBox MessageBox;
    public Inventory Inventory;

    public float CloseAfterSeconds = 1.75f;

    public override void OnPrimaryAction()
    {
        MessageBox.ShowForTime("You feel full", CloseAfterSeconds, gameObject);
        PlayerState.HungerThirst += 1.0f;

        Inventory.RemoveItem(this);
    }

}
