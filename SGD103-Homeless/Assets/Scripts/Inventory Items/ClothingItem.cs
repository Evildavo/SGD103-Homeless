using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClothingItem : InventoryItem
{
    public float Cleanliness = 1.0f;
    Color baseIconColour = Color.white;

    public float TimeCostToChange;
    public float DisplayAsDirtyBelow;
    public float DisplayAsFilthyBelow;
    public Color DirtyTextColour = Color.white;
    public Color FilthyTextColour = Color.white;
    [Header("This colour is multiplied with the base colour of the object")]
    public Color DirtyIconColour = Color.white;
    public Color FilthyIconColour = Color.white;

    // Call from derived.
    public override void OnPrimaryAction()
    {
        // Complete the cleanliness objective.
        Main.PlayerState.CompleteCleanlinessObjective();

        // Apply the time cost.
        Main.GameTime.SpendTime(TimeCostToChange);

        // Swap the selected clothes with what the player was wearing.
        if (Main.PlayerState.CurrentClothing)
        {
            Main.Inventory.AddItem(Main.PlayerState.CurrentClothing);
        }
        Main.Inventory.RemoveItem(this, false);
        transform.SetParent(Main.PlayerState.transform);
        Main.PlayerState.CurrentClothing = this;
    }
    
    // Call from derived.
    public override void OnCursorEnter()
    {
        if (Cleanliness < DisplayAsFilthyBelow)
        {
            Main.ItemDescription.ItemName.color = FilthyTextColour;
        }
        else if (Cleanliness < DisplayAsDirtyBelow)
        {
            Main.ItemDescription.ItemName.color = DirtyTextColour;
        }
        else
        {
            Main.ItemDescription.ItemName.color = Color.white;
        }
    }
    
    // Call from derived.
    public override void OnCursorExit()
    {
        Main.ItemDescription.ItemName.color = Color.white;
    }

    public void UpdateBaseColour()
    {
        baseIconColour = GetComponent<Image>().color;
    }
    
    void Start()
    {
        UpdateBaseColour();
    }

    void Update()
    {
        // Clothes get dirtier over time while worn.
        if (Main.PlayerState.CurrentClothing == this)
        {
            if (Cleanliness > 0.0f)
            {
                Cleanliness -= Main.PlayerState.ClothesDirtiedPerHourWorn * Main.GameTime.GameTimeDelta;
            }
            else
            {
                Cleanliness = 0.0f;
            }

            // Update cleanliness in player state.
            Main.PlayerState.CurrentClothingCleanliness = Cleanliness;
        }
        
        // Update the icon colour and text depending on filthiness.
        if (Cleanliness < DisplayAsFilthyBelow)
        {
            SubDescription = "Filthy";
            GetComponent<Image>().color = baseIconColour * FilthyIconColour;
        }
        else if (Cleanliness < DisplayAsDirtyBelow)
        {
            SubDescription = "Dirty";
            GetComponent<Image>().color = baseIconColour * DirtyIconColour;
        }
        else
        {
            SubDescription = "";
            GetComponent<Image>().color = baseIconColour;
        }
    }

}
