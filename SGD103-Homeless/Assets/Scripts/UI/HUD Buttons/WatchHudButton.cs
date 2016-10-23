using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WatchHudButton : MonoBehaviour {
    private bool isMouseOver = false;

    public Main Main;
    public WatchItem Watch;

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
    }

    public void OnClick()
    {
        Watch.gameObject.SetActive(true);
        Watch.OnPrimaryAction();
    }

    public void Update()
    {
        // Hide the watch if it's destroyed.
        if (!Watch)
        {
            gameObject.SetActive(false);
        }

        // Update the button label to show the time.
        if (isMouseOver)
        {
            var HudButtonLabel = Main.HudButtons.HudButtonLabel;
            HudButtonLabel.GetComponentInChildren<Text>().text = Main.GameTime.GetTimeAsString();
        }
    }
}
