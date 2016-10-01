using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;

    public Text ItemNameText;
    public Text ItemActionText;

    public InventoryItem GetItem()
    {
        return GetComponentInChildren<InventoryItem>();
    }

	void Start () {
        
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        if (ItemNameText)
        {
            ItemNameText.text = GetItem().ItemName;
            ItemActionText.text = GetItem().PrimaryActionDescription;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }

}
