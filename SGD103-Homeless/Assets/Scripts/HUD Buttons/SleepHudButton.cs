using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SleepHudButton : MonoBehaviour
{
    private bool isMouseOver = false;

    public Inventory Inventory;
    public Transform HudButtonLabel;
    public PlayerSleepManager SleepManager;
    public bool GiveSleepHint = true;
    public Color PoorSleepLabelColour = Color.white;
    public Color OkSleepLabelColour = Color.white;
    public Color GoodSleepLabelColour = Color.white;
    
    public void OnPointerEnter()
    {
        isMouseOver = true;
        HudButtonLabel.gameObject.SetActive(true);
        HudButtonLabel.GetComponentInChildren<Text>().text = "";
        Vector3 position = HudButtonLabel.transform.position;
        position.x = transform.position.x;
        HudButtonLabel.transform.position = position;
    }

    public void OnPointerExit()
    {
        isMouseOver = false;
        HudButtonLabel.gameObject.SetActive(false);
        HudButtonLabel.GetComponentInChildren<Text>().color = Color.white;
    }

    public void OnClick()
    {
        SleepManager.Sleep();
    }

    public void Update()
    {
        // Update the button label based on expected quality of sleep here.
        if (isMouseOver)
        {
            HudButtonLabel.GetComponentInChildren<Text>().text = "Sleep here";
            HudButtonLabel.GetComponentInChildren<Text>().color = Color.white;

            if (GiveSleepHint)
            {
                switch (SleepManager.SleepQualityHere)
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
        }
    }

}
