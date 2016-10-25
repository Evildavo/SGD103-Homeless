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
    public AlcoholItem AlcoholPrefab;

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
            OnWaterSelected, "Water", WaterPrefab.ItemValue, Main.PlayerState.CanAfford(WaterPrefab.ItemValue)));
        options.Add(new Menu.Option(
            OnBreadSelected, "Bread", BreadPrefab.ItemValue, Main.PlayerState.CanAfford(BreadPrefab.ItemValue)));
        options.Add(new Menu.Option(
            OnMandarinSelected, "Bag of Mandarins", MandarinPrefab.ItemValue, Main.PlayerState.CanAfford(MandarinPrefab.ItemValue)));
        options.Add(new Menu.Option(
            OnAppleSelected, "Apple", ApplePrefab.ItemValue, Main.PlayerState.CanAfford(ApplePrefab.ItemValue)));
        options.Add(new Menu.Option(
            OnPotatoChipsSelected, "Potato Chips", PotatoChipsPrefab.ItemValue, Main.PlayerState.CanAfford(PotatoChipsPrefab.ItemValue)));
        options.Add(new Menu.Option(
            OnBiscuitsSelected, "Biscuits", BiscuitsPrefab.ItemValue, Main.PlayerState.CanAfford(BiscuitsPrefab.ItemValue)));
        options.Add(new Menu.Option(
            OnChocolateBarSelected, "Chocolate Bar", ChocolateBarPrefab.ItemValue, Main.PlayerState.CanAfford(ChocolateBarPrefab.ItemValue)));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Main.Menu.Show(options);
    }

    public void OpenOutdoorItemMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnBuySleepingBag, "Buy a \"Pillow-Time\"(tm) sleeping bag", SleepingBagPrefab.ItemValue, Main.PlayerState.CanAfford(SleepingBagPrefab.ItemValue)));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Main.Menu.Show(options);
    }

    public void OpenLiquorItemMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnBuyAlcohol, "Buy nice wine", AlcoholPrefab.ItemValue, Main.PlayerState.CanAfford(AlcoholPrefab.ItemValue)));
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
            Main.PlayerState.Money -= WaterPrefab.ItemValue;

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
            Main.PlayerState.Money -= BreadPrefab.ItemValue;

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
            Main.PlayerState.Money -= MandarinPrefab.ItemValue;

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
            Main.PlayerState.Money -= ApplePrefab.ItemValue;

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
            Main.PlayerState.Money -= PotatoChipsPrefab.ItemValue;

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
            Main.PlayerState.Money -= BiscuitsPrefab.ItemValue;

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
            Main.PlayerState.Money -= ChocolateBarPrefab.ItemValue;

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
            Main.PlayerState.Money -= SleepingBagPrefab.ItemValue;

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
            Main.PlayerState.Money -= AlcoholPrefab.ItemValue;

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
