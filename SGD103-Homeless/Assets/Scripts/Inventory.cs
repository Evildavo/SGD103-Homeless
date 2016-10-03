using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {
    private bool isAwake = true;
    private float timeAtWake;
    private Vector3 lastMousePosition;
    private int lastMouseY;

    public Transform ItemDescription;
    public Transform SlotContainer;

    public float HideAfterSeconds = 1.0f;
    public int DeadZonePixels = 25;
    
	void Start () {
	    
	}

    void Update()
    {
        // Wake up the inventory if the mouse moved or is over an item.
        if (ItemDescription.gameObject.activeInHierarchy ||
            Mathf.Abs(lastMousePosition.x - Input.mousePosition.x) > DeadZonePixels ||
            Mathf.Abs(lastMousePosition.y - Input.mousePosition.y) > DeadZonePixels)
        {
            isAwake = true;
            timeAtWake = Time.time;
            foreach (Transform child in SlotContainer.transform)
            {
                child.gameObject.SetActive(true);
            }
            lastMousePosition = Input.mousePosition;
        }

        // After time hide the inventory.
        if (isAwake && Time.time - timeAtWake > HideAfterSeconds)
        {
            isAwake = false;
            foreach (Transform child in SlotContainer.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

}
