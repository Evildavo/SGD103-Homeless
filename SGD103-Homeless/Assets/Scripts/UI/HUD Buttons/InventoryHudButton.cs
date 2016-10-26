using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryHudButton : MonoBehaviour {

    public Main Main;

    [ReadOnly]
    public bool IsCursorOver = false;

    public void OnPointerEnter()
    {
        var HudButtonLabel = Main.HudButtons.HudButtonLabel;
        IsCursorOver = true;
        HudButtonLabel.gameObject.SetActive(true);
        HudButtonLabel.GetComponentInChildren<Text>().text = "Open/Close Inventory (i)";
        Vector3 position = HudButtonLabel.transform.position;
        position.x = transform.position.x;
        HudButtonLabel.transform.position = position;
    }

    public void OnPointerExit()
    {
        var HudButtonLabel = Main.HudButtons.HudButtonLabel;
        IsCursorOver = false;
        HudButtonLabel.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        var Inventory = Main.Inventory;
        if (Inventory.InSellMode && Main.PlayerState.CurrentTrigger != null)
        {
            // Close the trigger.
            Main.PlayerState.CurrentTrigger.Close();
            Inventory.Show();
        }
        else
        {
            // Toggle inventory.
            bool inventoryHidden = Inventory.IsHidden();
            if (inventoryHidden)
            {
                Inventory.Show();
            }
            else
            {
                Inventory.Hide();
            }
        }
    }

}
