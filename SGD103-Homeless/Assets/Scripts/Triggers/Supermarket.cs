using UnityEngine;
using System.Collections.Generic;


public class Supermarket : MonoBehaviour
{    
    public GameTime GameTime;
    public Trigger Trigger;
    public Menu Menu;
    public MessageBox MessageBox;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public PlayerSleepManager SleepManager;
    public InventoryItemDescription ItemDescription;
    public SleepHudButton SleepHudButton;
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
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenFoodMenu, "Buy food"));
        options.Add(new Menu.Option(OpenOutdoorItemMenu, "Buy outdoor equipment"));
        options.Add(new Menu.Option(OnExitSelected, "Exit"));

        Menu.Show(options);
    }

    public void OpenFoodMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnWaterSelected, "Water", WaterCost, PlayerState.CanAfford(WaterCost)));
        options.Add(new Menu.Option(
            OnBreadSelected, "Bread", BreadCost, PlayerState.CanAfford(BreadCost)));
        options.Add(new Menu.Option(
            OnMandarinSelected, "Mandarin", MandarinCost, PlayerState.CanAfford(MandarinCost)));
        options.Add(new Menu.Option(
            OnAppleSelected, "Apple", AppleCost, PlayerState.CanAfford(AppleCost)));
        options.Add(new Menu.Option(
            OnPotatoChipsSelected, "Potato Chips", PotatoChipsCost, PlayerState.CanAfford(PotatoChipsCost)));
        options.Add(new Menu.Option(
            OnBiscuitsSelected, "Biscuits", BiscuitsCost, PlayerState.CanAfford(BiscuitsCost)));
        options.Add(new Menu.Option(
            OnChocolateBarSelected, "Chocolate Bar", ChocolateBarCost, PlayerState.CanAfford(ChocolateBarCost)));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Menu.Show(options);
    }

    public void OpenOutdoorItemMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(
            OnBuySleepingBag, "Buy a \"Pillow-Time\"(tm) sleeping bag", SleepingBagCost, PlayerState.CanAfford(SleepingBagCost)));
        options.Add(new Menu.Option(OnBackSelected, "Back"));

        Menu.Show(options);
    }

    public void OnBackSelected()
    {
        OpenMainMenu();
    }
    
    public void OnWaterSelected()
    {
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= WaterCost;

            // Add item.
            FoodItem item = Instantiate(WaterPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenFoodMenu();
    }

    public void OnBreadSelected()
    {
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= BreadCost;

            // Add item.
            FoodItem item = Instantiate(BreadPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenFoodMenu(); 
    }

    public void OnMandarinSelected()
    {  
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= MandarinCost;

            // Add item.
            FoodItem item = Instantiate(MandarinPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenFoodMenu();
    }

    public void OnAppleSelected()
    {
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= AppleCost;

            // Add item.
            FoodItem item = Instantiate(ApplePrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenFoodMenu();
    }

    public void OnPotatoChipsSelected()
    {
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= PotatoChipsCost;

            // Add item.
            FoodItem item = Instantiate(PotatoChipsPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenFoodMenu();
    }

    public void OnBiscuitsSelected()
    {
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= BiscuitsCost;

            // Add item.
            FoodItem item = Instantiate(BiscuitsPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenFoodMenu();
    }

    public void OnChocolateBarSelected()
    {
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= ChocolateBarCost;

            // Add item.
            FoodItem item = Instantiate(ChocolateBarPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenFoodMenu();
    }

    public void OnBuySleepingBag()
    {
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= SleepingBagCost;

            // Add item.
            SleepItem item = Instantiate(SleepingBagPrefab);
            item.SleepManager = SleepManager;
            item.ItemDescription = ItemDescription;
            item.SleepHudButton = SleepHudButton;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        OpenOutdoorItemMenu();
    }


    public void OnExitSelected()
    {
        reset();
    }

    public void OnTrigger()
    {
        OpenMainMenu();
        Debug.Log("Yeah Boi");
    }
    public void OnPlayerExit()
    {
        Menu.Hide();
        reset();
    }

    void reset()
    {
        Menu.Hide();
        if (Trigger)
        {
            Trigger.Reset();
        }
    }

    public void OnTriggerUpdate()
    {
        if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }


}
