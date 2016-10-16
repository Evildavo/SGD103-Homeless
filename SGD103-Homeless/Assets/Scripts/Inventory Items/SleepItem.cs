﻿using UnityEngine;
using System.Collections;

public class SleepItem : InventoryItem
{
    private bool isOver = false;

    public PlayerSleepManager SleepManager;
    public InventoryItemDescription ItemDescription;
    public SleepHudButton SleepHudButton;

    public override void OnPrimaryAction()
    {
        SleepManager.Sleep();
    }

    public override void OnCursorEnter()
    {
        isOver = true;
    }

    public override void OnCursorExit()
    {
        ItemDescription.ItemAction.color = Color.white;
        isOver = false;
    }

    void Update()
    {
        if (isOver && SleepHudButton && SleepHudButton.GiveSleepHint)
        {
            PrimaryActionDescription = "Sleep here";
            switch (SleepManager.SleepQualityHere)
            {
                case PlayerSleepManager.SleepQualityEnum.POOR:
                    PrimaryActionDescription += " (bad)";
                    ItemDescription.ItemAction.color = SleepHudButton.PoorSleepLabelColour;
                    break;
                case PlayerSleepManager.SleepQualityEnum.OK:
                    PrimaryActionDescription += " (ok)";
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
