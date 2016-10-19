using UnityEngine;
using System.Collections;

public class FoodItem : InventoryItem
{
    public MessageBox MessageBox;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public float HungerSatietyBenefit = 0.1f;

    public override void OnPrimaryAction()
    {
        PlayerState.HungerThirstSatiety += HungerSatietyBenefit;

        if (PlayerState.HungerThirstSatiety > 0.6f)
        { 
            MessageBox.ShowForTime("You feel full", 2.0f, gameObject);
        }
        else if (PlayerState.HungerThirstSatiety > 0.4f)
        {
            MessageBox.ShowForTime("You needed that", 2.0f, gameObject);
        }
        else
        {
            MessageBox.ShowForTime("You're still hungry", 2.0f, gameObject);
        }
        Inventory.RemoveItem(this);
    }



    void Update()
    {
    }

}
