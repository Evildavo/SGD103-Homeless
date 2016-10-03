using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;

    public InventoryItemDescription ItemDescription;

    public InventoryItem GetItem()
    {
        return GetComponentInChildren<InventoryItem>(true);
    }
    
    void Start()
    {
        ItemDescription.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        InventoryItem item = GetItem();
        if (item && ItemDescription)
        {
            // Update text.
            ItemDescription.gameObject.SetActive(true);
            ItemDescription.ItemName.text = item.ItemName;
            ItemDescription.ItemAction.text = item.PrimaryActionDescription;

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
        InventoryItem item = GetItem();
        if (isOver && item)
        {
            item.OnPrimaryAction();
        }
    }

}
