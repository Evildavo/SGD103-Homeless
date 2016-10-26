using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private float timeAtPreviewStart;
    private bool isHidden = false;
    private List<OnItemAdded> onItemAddedCallbacks = new List<OnItemAdded>();

    public Main Main;
    public Transform SlotContainer;
    public Transform ItemContainer;

    [Tooltip("A value of 0 will disable hiding")]
    public int DeadZonePixels = 25;
    public bool CloseOnItemUse = true;
    public bool CloseOnClickOutside = true;
    public bool HiddenAtStart = true;
    public float PreviewTime = 2.0f;
    public bool OpenPreviewOnItemAdded = true;

    [Space(20)]
    public bool InSellMode = false;
    [ReadOnly]
    public bool IsPreviewing = false;
    [ReadOnly]
    public bool IsInventoryFull = false;
    public OnItemSold ItemSoldCallback;
    
    // Callback for when an item is added to the inventory.
    public delegate void OnItemAdded(InventoryItem item);

    // Callback for when the players sells an item in sell mode.
    public delegate void OnItemSold(InventoryItem item);

    // Registers the given function to be called when an item is added to the inventory.
    public void RegisterOnItemAdded(OnItemAdded callback)
    {
        onItemAddedCallbacks.Add(callback);
    }

    // Enters mode for selling items.
    // Uses the given callback for when requests to sell an item.
    public void EnterSellMode(OnItemSold callback)
    {
        InSellMode = true;
        ItemSoldCallback = callback;

        // Show the inventory in sell mode.
        IsPreviewing = false;
        isHidden = false;
        Main.ItemDescription.gameObject.SetActive(false);
        Main.DiscardHint.gameObject.SetActive(false);
        Main.HudButtons.OpenInventoryMode();
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            slot.Show();
        }
        ResetSellModeItemDescription();
    }

    // Exit sell mode.
    public void ExitSellMode()
    {
        InSellMode = false;
        Main.SellItemDescription.gameObject.SetActive(false);
        Hide();
    }

    public void ResetSellModeItemDescription()
    {
        // Set to default.
        Main.SellItemDescription.gameObject.SetActive(true);
        Main.SellItemDescription.ItemName.text = "Sell an item";
        Main.SellItemDescription.ItemValue.text = "";
        Main.SellItemDescription.GetComponent<Image>().enabled = true;

        // Move description text to the default position.
        Vector3 position = Main.SellItemDescription.transform.position;
        position.x = Main.SellItemDescription.CentrePosition.transform.position.x;
        Main.SellItemDescription.transform.position = position;
    }

    // Shows the inventory.
    public void Show()
    {
        IsPreviewing = false;
        isHidden = false;
        Main.ItemDescription.ItemName.text = "";
        Main.ItemDescription.ItemAction.text = "";
        Main.ItemDescription.GetComponent<Image>().enabled = false;
        Main.ItemDescription.gameObject.SetActive(true);
        Main.DiscardHint.gameObject.SetActive(false);
        Main.HudButtons.OpenInventoryMode();
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            slot.Show();
        }
    }

    // Temporarily shows the inventory before closing automatically.
    // Doesn't work if the inventory was already open (not on a preview).
    public void ShowPreview()
    {
        timeAtPreviewStart = Time.time;
        if (isHidden)
        {
            Show();
            IsPreviewing = true;
        }
    }

    // Hides the inventory.
    public void Hide()
    {
        isHidden = true;
        Main.ItemDescription.gameObject.SetActive(false);
        Main.DiscardHint.gameObject.SetActive(false);
        Main.HudButtons.OpenRegularMode();
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            slot.Hide();
        }
    }

    // Returns true if the inventory is hidden.
    public bool IsHidden()
    {
        return isHidden;
    }
    
    // Returns true if the inventory has the given item.
    // They're compared just using ItemName.
    public bool HasItem(InventoryItem item)
    {
        foreach (InventoryItem existingItem in ItemContainer.GetComponentsInChildren<InventoryItem>(true))
        {
            if (item.ItemName == existingItem.ItemName)
            {
                return true;
            }
        }
        return false;
    }
    
    // Instantiate the item before calling if it's a prefab.
    public void AddItem(InventoryItem item)
    {
        if (IsInventoryFull)
        {
            Debug.LogWarning("Inventory is full.");
            return;
        }

        // Open inventory preview.
        if (OpenPreviewOnItemAdded)
        {
            ShowPreview();
        }

        // Add to next available free slot.
        int i = 0;
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.Item)
            {
                item.InventoryIndex = i;
                item.transform.position = slot.transform.position;
                item.transform.SetParent(ItemContainer, true);
                item.transform.localScale = slot.transform.localScale;
                slot.Item = item;
                if (isHidden)
                {
                    slot.Hide();
                }
                break;
            }
            i++;
        }

        // Notify listeners.
        foreach (OnItemAdded callback in onItemAddedCallbacks)
        {
            callback(item);
        }        
    }
    
    // Removes the item and moves other items to fill the gaps.
    // Warning: Currently also destroys the object.
    public void RemoveItem(InventoryItem item)
    {
        InventorySlot[] inventorySlots = SlotContainer.GetComponentsInChildren<InventorySlot>(true);
        InventorySlot slot = inventorySlots[item.InventoryIndex];

        // Move items down to fill the slot.
        for (var i = item.InventoryIndex; i < inventorySlots.Length; i++)
        {
            if (i == inventorySlots.Length - 1)
            {
                inventorySlots[i].Item = null;
            }
            else
            {
                InventoryItem itemAbove = inventorySlots[i + 1].Item;
                if (itemAbove)
                {
                    itemAbove.InventoryIndex = i;
                    itemAbove.transform.position = inventorySlots[i].transform.position;
                    inventorySlots[i].Item = itemAbove;
                }
                else
                {
                    inventorySlots[i].Item = null;
                }
            }
        }
        
        // Destroy the object.
        Destroy(item.gameObject);

        // Update the description for the slot now under the cursor.
        if (slot.Item)
        {
            slot.OnPointerEnter(null);
        }
        else
        {
            Main.ItemDescription.gameObject.SetActive(false);
            Main.DiscardHint.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (HiddenAtStart)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    
    void Update()
    {
        // Open inventory on "i" key.
        if (Input.GetKeyDown("i"))
        {
            if (InSellMode && Main.PlayerState.CurrentTrigger != null)
            {
                // Close the trigger.
                Main.PlayerState.CurrentTrigger.Close();
                Show();
            }
            else
            {
                // Toggle inventory.
                bool inventoryHidden = IsHidden();
                if (inventoryHidden)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        // Check if inventory is full.
        IsInventoryFull = true;
        foreach (InventorySlot slot in SlotContainer.GetComponentsInChildren<InventorySlot>(true))
        {
            if (!slot.Item)
            {
                IsInventoryFull = false;
            }
        }

        // Handle sell mode.
        if (InSellMode)
        {
            if (IsHidden())
            {
                Show();
            }
        }
        else
        {
            // Handle previewing.
            if (IsPreviewing)
            {
                // Keep preview open if the cursor is over an item.
                if (Main.ItemDescription.gameObject.activeInHierarchy && Main.ItemDescription.ItemName.text != "")
                {
                    timeAtPreviewStart = Time.time;
                }

                // Close the inventory after preview time has passed.
                if (Time.time - timeAtPreviewStart > PreviewTime)
                {
                    IsPreviewing = false;
                    Hide();
                }
            }

            // Close if the mouse was clicked while not over an item.
            if (CloseOnClickOutside && Input.GetButtonDown("Primary") &&
                (!Main.ItemDescription.gameObject.activeInHierarchy || Main.ItemDescription.ItemName.text == "") &&
                !Main.HudButtons.InventoryHudButton.IsCursorOver)
            {
                Hide();
            }
        }
    }

}
