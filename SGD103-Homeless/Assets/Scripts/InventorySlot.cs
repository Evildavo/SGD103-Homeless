using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;

    public InventoryItemDescription ItemDescription;
    public Inventory Inventory;
    public InventoryItem Item;
    
    void Start()
    {
        ItemDescription.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (Item)
        {
            Item.Show();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (Item)
        {
            Item.Hide();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        if (Item && ItemDescription)
        {
            // Update text.
            ItemDescription.gameObject.SetActive(true);
            ItemDescription.ItemName.text = Item.ItemName;
            ItemDescription.ItemAction.text = Item.PrimaryActionDescription;
            ItemDescription.GetComponent<Image>().enabled = true;

            // Move description text to the slot.
            if (ItemDescription)
            {
                Vector3 position = ItemDescription.transform.position;
                position.x = transform.position.x;
                ItemDescription.transform.position = position;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;

        // Hide description text.
        ItemDescription.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (isOver && Item)
        {
            Item.OnPrimaryAction();
            if (Inventory.CloseOnItemUse)
            {
                ItemDescription.ItemName.text = "";
                ItemDescription.ItemAction.text = "";
                ItemDescription.GetComponent<Image>().enabled = false;
                Inventory.Hide();
            }
        }
    }

}
