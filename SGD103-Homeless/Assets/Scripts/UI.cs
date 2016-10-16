using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {
    
    public HudButtons HudButtons;
    public Inventory Inventory;
    public Transform StatPanel;
    public Transform MoneyPanel;

    // Hides the game UI.
    public void Hide()
    {
        HudButtons.gameObject.SetActive(false);
        Inventory.Hide();
        StatPanel.gameObject.SetActive(false);
        MoneyPanel.gameObject.SetActive(false);
    }

    // Shows the game UI.
    public void Show()
    {
        HudButtons.gameObject.SetActive(true);
        StatPanel.gameObject.SetActive(true);
        MoneyPanel.gameObject.SetActive(true);
    }

    void Start () {
	
	}
	
	void Update () {
	
	}
}
