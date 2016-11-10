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
    public float FadeToBlackTime = 1.0f;
    public float FadeInFromBlackTime = 1.0f;

    // Call from derived.
    public override void OnPrimaryAction()
    {
        // Get changed if we're not in public.
        if (Main.PlayerState.IsInPrivate)
        {
            // Complete the cleanliness objective.
            Main.PlayerState.CompleteCleanlinessObjectives();

            // Fade to black.
            Main.ScreenFader.fadeTime = FadeToBlackTime;
            Main.ScreenFader.fadeIn = false;
            Invoke("OnFadeOutComplete", FadeInFromBlackTime);

            // Hide UI and switch to modal mode.
            Main.UI.Hide();
            Main.UI.EnableModalMode();
        }
        else
        {
            Main.MessageBox.ShowForTime("You need a private place to get changed");
        }
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

    void OnFadeOutComplete()
    {
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

        // Show UI and exit modal mode.
        Main.UI.Show();
        Main.UI.DisableModalMode();
        Main.PlayerState.CurrentTrigger.ActivateTrigger();
        if (Main.PlayerState.CurrentTrigger)
        {
        }
        
        // Fade in from black.
        Main.ScreenFader.fadeTime = FadeInFromBlackTime;
        Main.ScreenFader.fadeIn = true;

        Invoke("onFadeInComplete", FadeInFromBlackTime);
    }

    void onFadeInComplete()
    {
    }

}
