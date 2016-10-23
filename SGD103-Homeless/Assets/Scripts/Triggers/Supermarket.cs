using UnityEngine;
using System.Collections.Generic;


public class Supermarket : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public JobLocation JobLocation;
    public FoodItem WaterPrefab;
    public FoodItem BreadPrefab;
    public FoodItem MandarinPrefab;
    public FoodItem ApplePrefab;
    public FoodItem PotatoChipsPrefab;
    public FoodItem BiscuitsPrefab;
    public FoodItem ChocolateBarPrefab;
    public SleepItem SleepingBagPrefab;

    public float WaterCost;
    public float BreadCost;
    public float MandarinCost;
    public float AppleCost;
    public float PotatoChipsCost;
    public float BiscuitsCost;
    public float ChocolateBarCost;
    public float SleepingBagCost;


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

    public void OpenFoodMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnWaterSelected, "Water", WaterCost, Main.PlayerState.CanAfford(WaterCost)));
        options.Add(new Menu.Option(
            OnBreadSelected, "Bread", BreadCost, Main.PlayerState.CanAfford(BreadCost)));
        options.Add(new Menu.Option(
            OnMandarinSelected, "Bag of Mandarins", MandarinCost, Main.PlayerState.CanAfford(MandarinCost)));
        options.Add(new Menu.Option(
            OnAppleSelected, "Apple", AppleCost, Main.PlayerState.CanAfford(AppleCost)));
        options.Add(new Menu.Option(
            OnPotatoChipsSelected, "Potato Chips", PotatoChipsCost, Main.PlayerState.CanAfford(PotatoChipsCost)));
        options.Add(new Menu.Option(
            OnBiscuitsSelected, "Biscuits", BiscuitsCost, Main.PlayerState.CanAfford(BiscuitsCost)));
        options.Add(new Menu.Option(
            OnChocolateBarSelected, "Chocolate Bar", ChocolateBarCost, Main.PlayerState.CanAfford(ChocolateBarCost)));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Main.Menu.Show(options);
    }

    public void OpenOutdoorItemMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnBuySleepingBag, "Buy a \"Pillow-Time\"(tm) sleeping bag", SleepingBagCost, Main.PlayerState.CanAfford(SleepingBagCost)));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Main.Menu.Show(options);
    }

    public void OnBackSelected()
    {
        OpenMainMenu();
    }
    
    public void OnWaterSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= WaterCost;

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

    public void OnBreadSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= BreadCost;

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
            Main.PlayerState.Money -= MandarinCost;

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
            Main.PlayerState.Money -= AppleCost;

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
            Main.PlayerState.Money -= PotatoChipsCost;

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
            Main.PlayerState.Money -= BiscuitsCost;

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
            Main.PlayerState.Money -= ChocolateBarCost;

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
            Main.PlayerState.Money -= SleepingBagCost;

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
