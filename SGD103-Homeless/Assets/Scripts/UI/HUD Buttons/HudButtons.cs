using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudButtons : MonoBehaviour {
    
    public Transform HudButtonLabel;
    public InventoryHudButton InventoryHudButton;
    public InventoryHudButton CloseInventoryHudButton;
    public SleepHudButton SleepHudButton;
    public WatchHudButton WatchHudButton;

    void Start()
    {
        HudButtonLabel.gameObject.SetActive(false);
        OpenRegularMode();
    }

    // Ensures hud is disabled in correct state.
    public void Hide()
    {
        gameObject.SetActive(false);
        InventoryHudButton.OnPointerExit();
        SleepHudButton.OnPointerExit();
        WatchHudButton.OnPointerExit();
    }

    // Changes the Hud button to inventory open mode.
    public void OpenInventoryMode()
    {
        CloseInventoryHudButton.gameObject.SetActive(true);
        InventoryHudButton.gameObject.SetActive(false);
        SleepHudButton.gameObject.SetActive(false);
        WatchHudButton.gameObject.SetActive(false);
        HudButtonLabel.gameObject.SetActive(false);
    }

    // Changes the Hud button layout to regular mode.
    public void OpenRegularMode()
    {
        CloseInventoryHudButton.gameObject.SetActive(false);
        InventoryHudButton.gameObject.SetActive(true);
        SleepHudButton.gameObject.SetActive(true);
        WatchHudButton.gameObject.SetActive(true);
    }

}
