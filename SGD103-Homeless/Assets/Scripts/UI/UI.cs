using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {

    public Main Main;

    // Hides the game UI.
    public void Hide()
    {
        if (Main.PlayerState.CurrentTrigger != null)
        {
            Main.PlayerState.CurrentTrigger.Close();
        }
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

    void Start () {
	
	}
	
	void Update () {
	
	}
}
