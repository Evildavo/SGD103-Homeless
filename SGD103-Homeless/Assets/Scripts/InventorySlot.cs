using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;

    public InventoryItemDescription ItemDescription;
    public InventorySellModeItemDescription SellModeItemDescription;
    public DiscardHint DiscardHint;
    public Inventory Inventory;
    public InventoryItem Item;
    
    void Start()
    {
        ItemDescription.gameObject.SetActive(false);
        DiscardHint.gameObject.SetActive(false);
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

    void updateItemDescription()
    {
        if (Item)
        {
            if (Inventory.InSellMode)
            {
                // Update text.
                SellModeItemDescription.gameObject.SetActive(true);
                SellModeItemDescription.ItemName.text = Item.ItemName;
                SellModeItemDescription.ItemValue.text = "$" + Item.ItemValue.ToString("f2");
                SellModeItemDescription.GetComponent<Image>().enabled = true;

                // Move description text to the slot.
                Vector3 position = SellModeItemDescription.transform.position;
                position.x = transform.position.x;
                SellModeItemDescription.transform.position = position;
            }
            else
            {
                // Update text.
                ItemDescription.gameObject.SetActive(true);
                ItemDescription.ItemName.text = Item.ItemName;
                ItemDescription.ItemAction.text = Item.PrimaryActionDescription;
                ItemDescription.GetComponent<Image>().enabled = true;

                // Show the discard hint.
                DiscardHint.gameObject.SetActive(Item.CanBeDiscarded);

                // Move description text to the slot.
                Vector3 position = ItemDescription.transform.position;
                position.x = transform.position.x;
                ItemDescription.transform.position = position;
            }
        }
        else
        {
            ItemDescription.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        if (Item && ItemDescription)
        {
            updateItemDescription();
            Item.OnCursorEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;

        if (Inventory.InSellMode)
        {
            Inventory.ResetSellModeItemDescription();
        }
        else
        {
            // Hide description text.
            ItemDescription.gameObject.SetActive(false);
            DiscardHint.gameObject.SetActive(false);
        }

        // Update inventory item.
        if (Item)
        {
            Item.OnCursorExit();
        }
    }

    public void OnClick()
    {
        updateItemDescription();
        
        if (isOver && Item)
        {
            if (Inventory.InSellMode)
            {
                // Handle selling the item.
                Item.OnSellRequested();
                if (Item.CanBeSold)
                {
                    Inventory.ItemSoldCallback(Item);
                }
            }
            else
            {
                // Handle click.
                Item.OnPrimaryAction();
                if (Inventory.CloseOnItemUse)
                {
                    ItemDescription.gameObject.SetActive(false);
                    ItemDescription.ItemName.text = "";
                    ItemDescription.ItemAction.text = "";
                    ItemDescription.GetComponent<Image>().enabled = false;
                    DiscardHint.gameObject.SetActive(false);
                    Inventory.Hide();
                }
            }
        }
    }

}
