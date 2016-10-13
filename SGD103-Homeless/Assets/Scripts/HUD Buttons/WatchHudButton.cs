using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WatchHudButton : MonoBehaviour {
    private bool isMouseOver = false;

    public Inventory Inventory;
    public Transform HudButtonLabel;
    public GameTime GameTime;
    public WatchItem Watch;

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
    }

    public void OnClick()
    {
        Watch.gameObject.SetActive(true);
        Watch.OnPrimaryAction();
    }

    public void Update()
    {
        if (isMouseOver)
        {
            HudButtonLabel.GetComponentInChildren<Text>().text = GameTime.GetTimeAsString();
        }
    }
}
