using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class UI : MonoBehaviour {
    private bool inModalMode = false;

    public Main Main;

    public System.Action ReturnTo;
    public Trigger CurrentTrigger;
    
    // Hides the game UI.
    public void Hide()
    {
        Main.HudButtons.Hide();
        Main.Inventory.Hide();
        Main.Menu.Hide();
        Main.StatPanel.gameObject.SetActive(false);
        Main.MoneyPanel.gameObject.SetActive(false);
        Main.InteractPrompt.gameObject.SetActive(false);
    }

    // Shows the game UI.
    public void Show()
    {
        Main.HudButtons.gameObject.SetActive(true);
        Main.StatPanel.gameObject.SetActive(true);
        Main.MoneyPanel.gameObject.SetActive(true);
        Main.InteractPrompt.gameObject.SetActive(true);
    }

    public bool IsInModalMode()
    {
        return inModalMode;
    }

    public void EnableModalMode()
    {
        inModalMode = true;
        Main.PlayerCharacter.MovementEnabled = false;
    }

    public void DisableModalMode()
    {
        inModalMode = false;
        Main.PlayerCharacter.MovementEnabled = true;
    }

    void Start () {
	
	}
	
	void Update () {
	
	}
}
