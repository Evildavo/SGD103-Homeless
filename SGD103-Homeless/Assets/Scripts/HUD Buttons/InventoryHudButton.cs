using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryHudButton : MonoBehaviour {

    public Inventory Inventory;
    public Transform HudButtonLabel;
    public PlayerState PlayerState;

    [ReadOnly]
    public bool IsCursorOver = false;

    public void OnPointerEnter()
    {
        IsCursorOver = true;
        HudButtonLabel.gameObject.SetActive(true);
        HudButtonLabel.GetComponentInChildren<Text>().text = "Open/Close Inventory";
        Vector3 position = HudButtonLabel.transform.position;
        position.x = transform.position.x;
        HudButtonLabel.transform.position = position;
    }

    public void OnPointerExit()
    {
        IsCursorOver = false;
        HudButtonLabel.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (Inventory.InSellMode && PlayerState.CurrentTrigger != null)
        {
            // Close the trigger.
            PlayerState.CurrentTrigger.Close();
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
