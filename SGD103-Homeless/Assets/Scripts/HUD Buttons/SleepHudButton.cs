using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SleepHudButton : MonoBehaviour {

    public Inventory Inventory;
    public Transform HudButtonLabel;

    public void OnPointerEnter()
    {
        HudButtonLabel.gameObject.SetActive(true);
        HudButtonLabel.GetComponentInChildren<Text>().text = "Sleep here";
        Vector3 position = HudButtonLabel.transform.position;
        position.x = transform.position.x;
        HudButtonLabel.transform.position = position;
    }

    public void OnPointerExit()
    {
        HudButtonLabel.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        Debug.Log("Sleeping");
    }

}
