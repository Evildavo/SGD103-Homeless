using UnityEngine;
using System.Collections;

public class SleepItem : InventoryItem
{
    private bool isOver = false;
    
    public float ImprovesSleepQualityPercent = 0.0f;
    
    // Call in the base class if deriving.
    public override void OnPrimaryAction()
    {
        Main.SleepManager.Sleep(this);
    }

    // Call in the base class if deriving.
    public override void OnCursorEnter()
    {
        isOver = true;
        Update();
    }

    // Call in the base class if deriving.
    public override void OnCursorExit()
    {
        Main.ItemDescription.ItemAction.color = Color.white;
        isOver = false;
    }

    // Call in the base class if deriving.
    public virtual void Update()
    {
        if (isOver)
        {
            var SleepHudButton = Main.HudButtons.SleepHudButton;
            var ItemDescription = Main.ItemDescription;
            PrimaryActionDescription = "Click to Sleep";
            if (SleepHudButton && SleepHudButton.GiveSleepHint)
            {
                switch (Main.SleepManager.SleepQualityHere)
                {
                    case PlayerSleepManager.SleepQualityEnum.TERRIBLE:
                        PrimaryActionDescription += " (terrible)";
                        ItemDescription.ItemAction.color = SleepHudButton.PoorSleepLabelColour;
                        break;
                    case PlayerSleepManager.SleepQualityEnum.POOR:
                        PrimaryActionDescription += " (poor)";
                        ItemDescription.ItemAction.color = SleepHudButton.OkSleepLabelColour;
                        break;
                    case PlayerSleepManager.SleepQualityEnum.GOOD:
                        PrimaryActionDescription += " (good)";
                        ItemDescription.ItemAction.color = SleepHudButton.GoodSleepLabelColour;
                        break;
                }
            }
        }
    }
}
