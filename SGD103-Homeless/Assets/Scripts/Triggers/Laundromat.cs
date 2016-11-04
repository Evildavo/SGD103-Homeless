using UnityEngine;
using System.Collections.Generic;

public class Laundromat : MonoBehaviour
{ 
    public Main Main;
    public Trigger Trigger;

    public float CostToUse;
    public float TimeCostToUse;
    public float FadeToBlackTime = 2.0f;
    public float FadeInFromBlackTime = 2.0f;
    public bool IncludeClothesPlayerIsWearing = true;

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onWashClothes, "Wash and dry clothes", CostToUse, Main.PlayerState.CanAfford(CostToUse)));
        options.Add(new Menu.Option(OnExit, "Exit"));

        Main.Menu.Show(options);
    }
    
    public void onWashClothes()
    {
        // Pay money.
        Main.PlayerState.Money -= CostToUse;

        // Show washing message.
        Main.MessageBox.Show("Washing...", gameObject);

        // Fade to black.
        Main.ScreenFader.fadeTime = FadeToBlackTime;
        Main.ScreenFader.fadeIn = false;
        Invoke("OnFadeOutComplete", FadeInFromBlackTime);

        // Hide UI and switch to modal mode.
        Main.UI.Hide();
        Main.UI.EnableModalMode();
    }

    void OnFadeOutComplete()
    {
        // Apply time cost.
        Main.GameTime.SpendTime(TimeCostToUse);
        
        // Wash what the player is wearing.
        if (IncludeClothesPlayerIsWearing && Main.PlayerState.CurrentClothing)
        {
            Main.PlayerState.CurrentClothing.Cleanliness = 1.0f;
        }

        // Wash all clothes in the player inventory.
        bool hasClothesInInventory = false;
        foreach (ClothingItem item in Main.Inventory.ItemContainer.GetComponentsInChildren<ClothingItem>())
        {
            item.Cleanliness = 1.0f;
            hasClothesInInventory = true;
        }

        // Open the inventory to show the clean clothes.
        if (hasClothesInInventory)
        {
            Main.Inventory.ShowPreview();
        }

        // Show UI and exit modal mode.
        Main.UI.Show();
        Main.UI.DisableModalMode();
        
        // Fade in from black.
        Main.ScreenFader.fadeTime = FadeInFromBlackTime;
        Main.ScreenFader.fadeIn = true;

        // Show message that clothes were cleaned.
        Main.MessageBox.ShowForTime("Clothes washed", null, gameObject);

        Invoke("onFadeInComplete", FadeInFromBlackTime);
        reset();
    }

    void onFadeInComplete()
    {
    }

    void Start ()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnCloseRequested(reset);
    }
    	
	void OnTrigger () {
        OpenMainMenu();
    }

    void OnExit()
    {
        reset();
        Main.MessageBox.ShowNext();
    }

    void reset()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }

}
