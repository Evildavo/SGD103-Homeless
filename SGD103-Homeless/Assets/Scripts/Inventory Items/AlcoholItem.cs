﻿using UnityEngine;
using System.Collections;

public class AlcoholItem : MultiUseItem
{
    [UnityEngine.Serialization.FormerlySerializedAs("ThirstSatietyBenefitPerUse")]
    public float NutritionBenefitPerUse;
    public float MoraleBenefitPerUse;
    public float InebriationIncreasePerUse;
    public float AddictionIncreasePerUse;
    public float TimeCostPerUse;

    public override void OnPrimaryAction()
    {
        var PlayerState = Main.PlayerState;
        var MessageBox = Main.MessageBox;

        // Increase stats.
        PlayerState.ChangeNutrition(NutritionBenefitPerUse);
        PlayerState.ChangeMorale(MoraleBenefitPerUse, false);
        PlayerState.Inebriation += InebriationIncreasePerUse;
        PlayerState.Addiction += AddictionIncreasePerUse;
        
        // Apply time cost.
        Main.GameTime.SpendTime(TimeCostPerUse);

        PlayerState.CompleteDrinkAlcoholObjectives();

        // Show message depending on how inebriated the player is after drinking.
        if (PlayerState.Inebriation > 0.8f)
        {
            MessageBox.ShowForTime("You feel very good", null, gameObject);
        }
        else if (PlayerState.Inebriation > 0.6f)
        {
            MessageBox.ShowForTime("You feel pretty good", null, gameObject);
        }
        else if (PlayerState.Inebriation > 0.4f)
        {
            MessageBox.ShowForTime("You feel a little tipsy", null, gameObject);
        }
        else
        {
            MessageBox.ShowForTime("You feel a bit better", null, gameObject);
        }

        // Consume item.
        NumUses -= 1;
        if (NumUses == 0)
        {
            Main.Inventory.RemoveItem(this);
        }
    }

    new void Start()
    {
        base.Start();
    }

    void Update()
    {
        UpdateItemDescription();
    }
}
