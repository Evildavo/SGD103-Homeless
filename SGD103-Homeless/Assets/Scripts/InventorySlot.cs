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

	void Start()
    {
        
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        if (ItemNameText)
        {
            // Update text.
            ItemNameText.text = GetItem().ItemName;
            ItemActionText.text = GetItem().PrimaryActionDescription;

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
    }

}
