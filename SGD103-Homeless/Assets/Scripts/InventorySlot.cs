using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;

    public Transform ItemDescription;
    public Text ItemNameText;
    public Text ItemActionText;

    public InventoryItem GetItem()
    {
        return GetComponentInChildren<InventoryItem>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        InventoryItem item = GetItem();
        if (item && ItemNameText)
        {
            // Update text.
            ItemNameText.text = item.ItemName;
            ItemActionText.text = item.PrimaryActionDescription;
            ItemNameText.enabled = true;
            ItemActionText.enabled = true;

            // Move description text to the slot.
            if (ItemDescription)
            {
                Vector3 position = ItemDescription.position;
                position.x = transform.position.x;
                ItemDescription.position = position;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;

        // Hide description text.
        ItemNameText.enabled = false;
        ItemActionText.enabled = false;
    }

    public void OnClick()
    {
        InventoryItem item = GetItem();
        if (isOver && item)
        {
            item.OnPrimaryAction();
        }
    }

}
