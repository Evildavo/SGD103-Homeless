﻿using UnityEngine;
using System.Collections;

public class FoodItem : MultiUseItem
{
    public MessageBox MessageBox;
    public PlayerState PlayerState;
    public Inventory Inventory;

    public float HungerSatietyBenefitPerUse;

    public override void OnPrimaryAction()
    {
        // Increase stat.
        PlayerState.HungerThirstSatiety += HungerSatietyBenefitPerUse;

        // Show message depending on how full the player is after eating.
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

        // Consume item.
        NumUses -= 1;
        if (NumUses == 0)
        {
            Inventory.RemoveItem(this);
        }
    }
    
    void Update()
    {
        UpdateItemDescription();
    }

}
