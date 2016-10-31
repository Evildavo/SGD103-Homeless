using UnityEngine;
using System.Collections;

public class AntiDepressant : MultiUseItem
{
    bool hasTaken = false;
    int dayLastTaken;
    
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
            Main.MessageBox.ShowForTime("Only take ONE per day", 2.0f, gameObject);
        }
    }
    
    void Update()
    {
        UpdateItemDescription();        
    }

}
