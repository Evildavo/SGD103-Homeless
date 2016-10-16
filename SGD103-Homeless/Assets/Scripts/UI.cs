using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {
    
    public HudButtons HudButtons;
    public Inventory Inventory;
    public Transform StatPanel;
    public Transform MoneyPanel;
    public Transform TriggerName;
    public Transform InteractHint;

    // Hides the game UI.
    public void Hide()
    {
        HudButtons.Hide();
        Inventory.Hide();
        StatPanel.gameObject.SetActive(false);
        MoneyPanel.gameObject.SetActive(false);
        TriggerName.gameObject.SetActive(false);
        InteractHint.gameObject.SetActive(false);
}

    // Shows the game UI.
    public void Show()
    {
        HudButtons.gameObject.SetActive(true);
        StatPanel.gameObject.SetActive(true);
        MoneyPanel.gameObject.SetActive(true);
        TriggerName.gameObject.SetActive(true);
        InteractHint.gameObject.SetActive(true);
    }

    void Start () {
	
	}
	
	void Update () {
	
	}
}
