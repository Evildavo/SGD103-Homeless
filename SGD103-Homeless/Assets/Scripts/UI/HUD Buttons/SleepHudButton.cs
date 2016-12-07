using UnityEngine;
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
    public Color DisabledIconColour = Color.grey;
    public Color PoorSleepIconColour = Color.white;
    public Color OkSleepIconColour = Color.white;
    public Color GoodSleepIconColour = Color.white;

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
        if (!Main.PlayerState.CurrentTrigger && !Main.Splash.gameObject.activeInHierarchy)
        {
            Main.SleepManager.Sleep();
        }
    }

    public void Update()
    {
        if (!Main.SleepManager)
        {
            gameObject.SetActive(false);
        }

        // Grey out the option when the player is in a trigger or a splash screen.
        if (Main.PlayerState.CurrentTrigger || Main.Splash.gameObject.activeInHierarchy)
        {
            GetComponent<Image>().color = DisabledIconColour;
        }

        // Otherwise, set the colour based on expected sleep quality here.
        else
        {
            switch (Main.SleepManager.SleepQualityHere)
            {
                case PlayerSleepManager.SleepQualityEnum.TERRIBLE:
                    GetComponent<Image>().color = PoorSleepIconColour;
                    break;
                case PlayerSleepManager.SleepQualityEnum.POOR:
                    GetComponent<Image>().color = OkSleepIconColour;
                    break;
                case PlayerSleepManager.SleepQualityEnum.GOOD:
                    GetComponent<Image>().color = GoodSleepIconColour;
                    break;
                default:
                    GetComponent<Image>().color = Color.white;
                    break;
            }
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
                    case PlayerSleepManager.SleepQualityEnum.TERRIBLE:
                        HudButtonLabel.GetComponentInChildren<Text>().text += " (terrible)";
                        HudButtonLabel.GetComponentInChildren<Text>().color = PoorSleepLabelColour;
                        break;
                    case PlayerSleepManager.SleepQualityEnum.POOR:
                        HudButtonLabel.GetComponentInChildren<Text>().text += " (poor)";
                        HudButtonLabel.GetComponentInChildren<Text>().color = OkSleepLabelColour;
                        break;
                    case PlayerSleepManager.SleepQualityEnum.GOOD:
                        HudButtonLabel.GetComponentInChildren<Text>().text += " (ok)";
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
