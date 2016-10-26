using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudButtons : MonoBehaviour {
    Vector3 initialInventoryHudButtonPosition;
    Vector3 initialSleepHudButtonPosition;

    public Transform HudButtonLabel;
    public InventoryHudButton InventoryHudButton;
    public InventoryHudButton CloseInventoryHudButton;
    public SleepHudButton SleepHudButton;
    public WatchHudButton WatchHudButton;

    [Header("For moving items to centre when watch is gone")]
    public float MoveButtonsXWhenWatchGoneAmount = -60.0f; 

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
        HudButtonLabel.gameObject.SetActive(false);
        if (WatchHudButton.Watch)
        {
            WatchHudButton.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        initialInventoryHudButtonPosition = InventoryHudButton.transform.position;
        initialSleepHudButtonPosition = SleepHudButton.transform.position;

        HudButtonLabel.gameObject.SetActive(false);
        OpenRegularMode();
    }

    void Update()
    {
        // Move other items to centre when watch gone.
        if (!WatchHudButton.gameObject.activeInHierarchy)
        {
            {
                var position = initialInventoryHudButtonPosition;
                position.x += MoveButtonsXWhenWatchGoneAmount;
                InventoryHudButton.transform.position = position;
            }
            {
                var position = initialSleepHudButtonPosition;
                position.x += MoveButtonsXWhenWatchGoneAmount;
                SleepHudButton.transform.position = position;
            }
        }
        else
        {
            InventoryHudButton.transform.position = initialInventoryHudButtonPosition;
            SleepHudButton.transform.position = initialSleepHudButtonPosition;
        }
    }

}
