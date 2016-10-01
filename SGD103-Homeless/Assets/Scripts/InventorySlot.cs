using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;
    
    public InventoryItem GetItem()
    {
        return GetComponentInChildren<InventoryItem>();
    }

	void Start () {
        
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        Debug.Log(GetItem().ItemName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }

}
