using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {
    private bool isAwake = true;
    private float timeAtWake;
    private Vector3 lastMousePosition;
    private int lastMouseY;

    public Transform InventorySlotContainer;

    public float HideAfterSeconds = 3.0f;
    public int DeadZonePixels = 20;

    public InventorySlot[] GetSlots()
    {
        return InventorySlotContainer.GetComponentsInChildren<InventorySlot>();
    }
    
	void Start () {
	    
	}

    void Update()
    {
        // If mouse moved wake up inventory.
        if (Mathf.Abs(lastMousePosition.x - Input.mousePosition.x) > DeadZonePixels ||
            Mathf.Abs(lastMousePosition.y - Input.mousePosition.y) > DeadZonePixels)
        {
            isAwake = true;
            timeAtWake = Time.time;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            lastMousePosition = Input.mousePosition;
        }

        // After time hide the inventory.
        if (isAwake && Time.time - timeAtWake > HideAfterSeconds)
        {
            isAwake = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

}
