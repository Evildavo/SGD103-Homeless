using UnityEngine;
using System.Collections.Generic;


public class Supermarket : MonoBehaviour
{
    InventoryItem itemSelected;

    public Main Main;
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public JobLocation JobLocation;
    public List<InventoryItem> FoodMenuPrefabItems;
    public List<InventoryItem> EquipmentMenuPrefabItems;
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
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenFoodMenu, "Buy food"));
        options.Add(new Menu.Option(OpenOutdoorItemMenu, "Buy outdoor equipment"));
        options.Add(new Menu.Option(OpenLiquorItemMenu, "Buy liquor"));
        /*options.Add(new Menu.Option(OpenMedicineItemMenu, "Buy medicine"));*/
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

    void purchaseItemSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= WaterPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            FoodItem item = Instantiate(WaterPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenFoodMenu();
    }

    public void OpenFoodMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        foreach (InventoryItem item in FoodMenuPrefabItems)
        {
            itemSelected = item;
            options.Add(new Menu.Option(
                purchaseItemSelected, item.ItemName, item.ItemValue, Main.PlayerState.CanAfford(item.ItemValue)));
        }
        options.Add(new Menu.Option(OnBackSelected, "Back"));
        Main.Menu.Show(options);
    }

    public void OpenOutdoorItemMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnBuySleepingBag, "Buy a \"Pillow-Time\"(tm) sleeping bag", SleepingBagPrefab.GetItemValue(), Main.PlayerState.CanAfford(SleepingBagPrefab.GetItemValue())));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Main.Menu.Show(options);
    }

    public void OpenLiquorItemMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnBuyAlcohol, "Buy nice wine", AlcoholPrefab.GetItemValue(), Main.PlayerState.CanAfford(AlcoholPrefab.GetItemValue())));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Main.Menu.Show(options);
    }

    public void OpenMedicineItemMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnBuyAntiDepressant, "Buy anti-depressant", AntiDepressantPrefab.GetItemValue(), Main.PlayerState.CanAfford(AntiDepressantPrefab.GetItemValue())));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Main.Menu.Show(options);
    }

    public void OnBackSelected()
    {
        OpenMainMenu();
    }
    
    public void OnWaterSelected()
    {
    }

    public void OnBreadSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= BreadPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            FoodItem item = Instantiate(BreadPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenFoodMenu(); 
    }

    public void OnMandarinSelected()
    {  
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= MandarinPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            FoodItem item = Instantiate(MandarinPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenFoodMenu();
    }

    public void OnAppleSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= ApplePrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            FoodItem item = Instantiate(ApplePrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenFoodMenu();
    }

    public void OnPotatoChipsSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= PotatoChipsPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            FoodItem item = Instantiate(PotatoChipsPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenFoodMenu();
    }

    public void OnBiscuitsSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= BiscuitsPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            FoodItem item = Instantiate(BiscuitsPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenFoodMenu();
    }

    public void OnChocolateBarSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= ChocolateBarPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            FoodItem item = Instantiate(ChocolateBarPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenFoodMenu();
    }

    public void OnBuySleepingBag()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= SleepingBagPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            SleepItem item = Instantiate(SleepingBagPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenOutdoorItemMenu();
    }

    public void OnBuyAlcohol()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= AlcoholPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            AlcoholItem item = Instantiate(AlcoholPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenLiquorItemMenu();
    }

    public void OnBuyAntiDepressant()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= AntiDepressantPrefab.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            AntiDepressant item = Instantiate(AntiDepressantPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenMedicineItemMenu();
    }

    public void OnTrigger()
    {
        JobLocation.CheckForJob(true);
        OpenMainMenu();
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
