using UnityEngine;
using System.Collections;

public class WaterItem : InventoryItem
{
    public MessageBox MessageBox;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public float ThirstBenefit = 0.1f;

    public override void OnPrimaryAction()
    {
        MessageBox.ShowForTime("You drank me", 2.0f, gameObject);
        PlayerState.HungerThirstSatiety += ThirstBenefit;
        Inventory.RemoveItem(this);
    }



    void Update()
    {
    }

}
