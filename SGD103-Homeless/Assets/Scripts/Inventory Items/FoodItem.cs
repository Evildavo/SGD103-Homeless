using UnityEngine;
using System.Collections;

public class FoodItem : MultiUseItem
{

    [UnityEngine.Serialization.FormerlySerializedAs("HungerSatietyBenefitPerUse")]
    public float NutritionBenefitPerUse;

    public override void OnPrimaryAction()
    {
        var PlayerState = Main.PlayerState;
        var MessageBox = Main.MessageBox;

        // Increase stat.
        PlayerState.ChangeNutrition(NutritionBenefitPerUse);

        // Show message depending on how full the player is after eating.
        if (PlayerState.Nutrition + NutritionBenefitPerUse > 0.6f)
        { 
            MessageBox.ShowForTime("You feel full", 2.0f, gameObject);
        }
        else if (PlayerState.Nutrition + NutritionBenefitPerUse > 0.4f)
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
