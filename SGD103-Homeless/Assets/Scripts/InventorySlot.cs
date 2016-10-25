using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;

    public Main Main;
    public InventoryItem Item;
    
    void Start()
    {
        Main.ItemDescription.gameObject.SetActive(false);
        Main.DiscardHint.gameObject.SetActive(false);
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
            if (Main.Inventory.InSellMode)
            {
                // Update text.
                var SellItemDescription = Main.SellItemDescription;
                SellItemDescription.gameObject.SetActive(true);
                SellItemDescription.ItemName.text = Item.ItemName;
                SellItemDescription.ItemValue.text = "$" + Item.GetItemValue().ToString("f2");
                SellItemDescription.GetComponent<Image>().enabled = true;

                // Move description text to the slot.
                Vector3 position = SellItemDescription.transform.position;
                position.x = transform.position.x;
                SellItemDescription.transform.position = position;
            }
            else
            {
                // Update text.
                var ItemDescription = Main.ItemDescription;
                ItemDescription.gameObject.SetActive(true);
                ItemDescription.ItemName.text = Item.ItemName;
                ItemDescription.ItemAction.text = Item.PrimaryActionDescription;
                ItemDescription.GetComponent<Image>().enabled = true;

                // Show the discard hint.
                Main.DiscardHint.gameObject.SetActive(Item.CanBeDiscarded);

                // Move description text to the slot.
                Vector3 position = ItemDescription.transform.position;
                position.x = transform.position.x;
                ItemDescription.transform.position = position;
            }
        }
        else
        {
            Main.ItemDescription.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        if (Item && Main.ItemDescription)
        {
            updateItemDescription();
            Item.OnCursorEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;

        if (Main.Inventory.InSellMode)
        {
            Main.Inventory.ResetSellModeItemDescription();
        }
        else
        {
            // Hide description text.
            Main.ItemDescription.gameObject.SetActive(false);
            Main.DiscardHint.gameObject.SetActive(false);
        }

        // Update inventory item.
        if (Item)
        {
            Item.OnCursorExit();
        }
    }

    // Primary button press.
    public void OnClick()
    {
        updateItemDescription();
        
        if (isOver && Item)
        {
            if (Main.Inventory.InSellMode)
            {
                // Handle selling the item.
                Item.OnSellRequested();
                if (Item.CanBeSold)
                {
                    Main.Inventory.ItemSoldCallback(Item);
                }
            }
            else
            {
                // Handle click.
                Item.OnPrimaryAction();
                if (Main.Inventory.CloseOnItemUse)
                {
                    Main.ItemDescription.gameObject.SetActive(false);
                    Main.ItemDescription.ItemName.text = "";
                    Main.ItemDescription.ItemAction.text = "";
                    Main.ItemDescription.GetComponent<Image>().enabled = false;
                    Main.DiscardHint.gameObject.SetActive(false);
                    Main.Inventory.Hide();
                }
            }
        }
    }

    void Update()
    {
        // On secondary button press.
        if (isOver && Item && Input.GetButtonDown("Secondary"))
        {
            // Discard the item.
            if (Item.CanBeDiscarded)
            {
                Item.OnDiscard();
                Main.Inventory.RemoveItem(Item);
            }
        }
    }

}
