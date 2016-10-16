using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudButtons : MonoBehaviour {
    
    public Transform HudButtonLabel;
    public InventoryHudButton InventoryHudButton;
    public SleepHudButton SleepHudButton;
    public WatchHudButton WatchHudButton;

    void Start()
    {
        HudButtonLabel.gameObject.SetActive(false);
    }

    // Ensures hud is disabled in correct state.
    public void Hide()
    {
        gameObject.SetActive(false);
        InventoryHudButton.OnPointerExit();
        SleepHudButton.OnPointerExit();
        WatchHudButton.OnPointerExit();
    }

}
