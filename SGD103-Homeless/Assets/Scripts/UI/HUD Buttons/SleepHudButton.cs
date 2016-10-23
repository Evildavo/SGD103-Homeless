﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SleepHudButton : MonoBehaviour
{
    private bool isMouseOver = false;

    public Main Main;

    public bool GiveSleepHint = true;
    public Color PoorSleepLabelColour = Color.white;
    public Color OkSleepLabelColour = Color.white;
    public Color GoodSleepLabelColour = Color.white;
    
    public void OnPointerEnter()
    {
        var HudButtonLabel = Main.HudButtons.HudButtonLabel;
        isMouseOver = true;
        HudButtonLabel.gameObject.SetActive(true);
        HudButtonLabel.GetComponentInChildren<Text>().text = "";
        Vector3 position = HudButtonLabel.transform.position;
        position.x = transform.position.x;
        HudButtonLabel.transform.position = position;
    }

    public void OnPointerExit()
    {
        var HudButtonLabel = Main.HudButtons.HudButtonLabel;
        isMouseOver = false;
        HudButtonLabel.gameObject.SetActive(false);
        HudButtonLabel.GetComponentInChildren<Text>().color = Color.white;
    }

    public void OnClick()
    {
        Main.SleepManager.Sleep();
    }

    public void Update()
    {
        if (!Main.SleepManager)
        {
            gameObject.SetActive(false);
        }

        // Update the button label based on expected quality of sleep here.
        if (isMouseOver)
        {
            var HudButtonLabel = Main.HudButtons.HudButtonLabel;
            HudButtonLabel.GetComponentInChildren<Text>().color = Color.white;
            HudButtonLabel.GetComponentInChildren<Text>().text = "Sleep here";

            if (GiveSleepHint)
            {
                switch (Main.SleepManager.SleepQualityHere)
                {
                    case PlayerSleepManager.SleepQualityEnum.POOR:
                        HudButtonLabel.GetComponentInChildren<Text>().text += " (bad)";
                        HudButtonLabel.GetComponentInChildren<Text>().color = PoorSleepLabelColour;
                        break;
                    case PlayerSleepManager.SleepQualityEnum.OK:
                        HudButtonLabel.GetComponentInChildren<Text>().text += " (ok)";
                        HudButtonLabel.GetComponentInChildren<Text>().color = OkSleepLabelColour;
                        break;
                    case PlayerSleepManager.SleepQualityEnum.GOOD:
                        HudButtonLabel.GetComponentInChildren<Text>().text += " (good)";
                        HudButtonLabel.GetComponentInChildren<Text>().color = GoodSleepLabelColour;
                        break;
                }
            }
            
            SleepItem sleepItem = Main.SleepManager.GetBestSleepItem();
            if (sleepItem)
            {
                HudButtonLabel.GetComponentInChildren<Text>().text += "\nusing " + sleepItem.ItemName + "";
            }
        }
    }

}