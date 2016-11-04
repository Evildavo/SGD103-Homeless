using UnityEngine;
using System.Collections;

public class AntiDepressant : MultiUseItem
{
    bool hasTaken = false;
    int dayLastTaken;

    public float TimeCostPerUse;
    
    public override void OnPrimaryAction()
    {
        // Take if haven't taken today. 
        if (!hasTaken || Main.GameTime.Day != dayLastTaken)
        {
            // Treat depression for today.
            hasTaken = true;
            dayLastTaken = Main.GameTime.Day;
            Main.PlayerState.TreatDepressionToday();
            Main.PlayerState.HighlightMoraleStatForTime(true, 1.5f);
            
            // Apply time cost.
            Main.GameTime.SpendTime(TimeCostPerUse);

            // Consume item.
            NumUses -= 1;
            if (NumUses == 0)
            {
                Main.Inventory.RemoveItem(this);
            }
        }

        // Warn to only take once per day.
        else
        {
            Main.MessageBox.ShowForTime("Only take ONE per day", null, gameObject);
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
