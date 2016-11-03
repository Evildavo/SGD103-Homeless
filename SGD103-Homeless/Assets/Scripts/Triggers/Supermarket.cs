﻿using UnityEngine;
using System.Collections.Generic;


public class Supermarket : MonoBehaviour
{
    MenuEnum menu;

    enum MenuEnum
    {
        MAIN,
        FOOD,
        OUTDOOR_EQUIPMENT,
        LIQUOR,
        MEDICINE
    }
    
    public Main Main;
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public JobLocation JobLocation;
    public List<InventoryItem> FoodMenuPrefabItems;
    public List<InventoryItem> OutdoorEquipmentMenuPrefabItems;
    public List<InventoryItem> LiquorMenuPrefabItems;
    public List<InventoryItem> MedicineMenuPrefabItems;

    public float TimeCostToPurchaseItem;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    public void OpenMainMenu()
    {
        menu = MenuEnum.MAIN;
        List<Menu.Option> options = new List<Menu.Option>();
        if (FoodMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenFoodMenu, "Buy food"));
        }
        if (OutdoorEquipmentMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenOutdoorItemMenu, "Buy outdoor equipment"));
        }
        if (LiquorMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenLiquorItemMenu, "Buy liquor"));
        }
        if (MedicineMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenMedicineItemMenu, "Buy medicine"));
        }
        if (JobLocation.IsJobAvailableToday)
        {
            options.Add(new Menu.Option(ApplyForJob, "Apply for job"));
        }
        if (JobLocation.PlayerHasJobHere)
        {
            // Note that this is a ghost option to inform the player, not to be a useable menu item.
            string message = "Work (" + JobLocation.GetWorkTimeSummaryShort() + ")";
            options.Add(new Menu.Option(null, message, 0, false));
        }
        options.Add(new Menu.Option(Reset, "Exit"));

        Main.Menu.Show(options);
    }

    public void ApplyForJob()
    {
        JobLocation.ApplyForJob();
        OpenMainMenu();
    }
    
    // Adds the given purchase items to the given menu.
    void AddMenuOptions(List<InventoryItem> items, List<Menu.Option> options)
    {
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem item = items[i];

            // Use the purchase item name if available.
            string name = item.PurchaseItemName;
            if (name == "")
            {
                name = item.ItemName;
            }
            
            // Add the option. Uses a closure to hold the item selected parameter.
            options.Add(new Menu.Option(
                () => purchaseItemSelected(item), 
                name, item.GetItemValue(),
                Main.PlayerState.CanAfford(item.GetItemValue())));
        }
    }

    public void OpenFoodMenu()
    {
        menu = MenuEnum.FOOD;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(FoodMenuPrefabItems, options);
        options.Add(new Menu.Option(OnBackSelected, "Back"));
        Main.Menu.Show(options);
    }

    public void OpenOutdoorItemMenu()
    {
        menu = MenuEnum.OUTDOOR_EQUIPMENT;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(OutdoorEquipmentMenuPrefabItems, options);
        options.Add(new Menu.Option(OnBackSelected, "Back"));
        Main.Menu.Show(options);
    }

    public void OpenLiquorItemMenu()
    {
        menu = MenuEnum.LIQUOR;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(LiquorMenuPrefabItems, options);
        options.Add(new Menu.Option(OnBackSelected, "Back"));
        Main.Menu.Show(options);
    }

    public void OpenMedicineItemMenu()
    {
        menu = MenuEnum.MEDICINE;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(MedicineMenuPrefabItems, options);
        options.Add(new Menu.Option(OnBackSelected, "Back"));
        Main.Menu.Show(options);
    }

    public void OnBackSelected()
    {
        OpenMainMenu();
    }

    public void OnTrigger()
    {
        JobLocation.CheckForJob(true);
        OpenMainMenu();
    }

    void purchaseItemSelected(InventoryItem itemSelected)
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= itemSelected.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            InventoryItem item = Instantiate(itemSelected);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        updateMenu();
    }

    void updateMenu()
    {
        switch (menu)
        {
            case MenuEnum.MAIN:
                OpenMainMenu();
                break;
            case MenuEnum.FOOD:
                OpenFoodMenu();
                break;
            case MenuEnum.OUTDOOR_EQUIPMENT:
                OpenOutdoorItemMenu();
                break;
            case MenuEnum.LIQUOR:
                OpenLiquorItemMenu();
                break;
            case MenuEnum.MEDICINE:
                OpenMedicineItemMenu();
                break;
        }
    }

    void Reset()
    {
        Main.Menu.Hide();
        Main.MessageBox.ShowNext();
        if (Trigger)
        {
            Trigger.Reset(Trigger.IsEnabled);
        }
    }

    public void OnTriggerUpdate()
    {
        // Show warning that job is about to start.
        if (JobLocation.CanWorkNow)
        {
            Reset();
            Main.MessageBox.ShowForTime("Work is about to start.", 2.0f, gameObject);
        }
    }

    public void Update()
    {
        // Switch to the job trigger when it's time to start work.
        if (JobLocation.CanWorkNow)
        {
            Trigger.IsEnabled = false;
            JobTrigger.IsEnabled = true;
        }
        else
        {
            Trigger.IsEnabled = true;
            JobTrigger.IsEnabled = false;
        }
    }

}
