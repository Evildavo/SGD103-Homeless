using UnityEngine;
using System.Collections.Generic;


public class Supermarket : MonoBehaviour {

    private List<Menu.Option> mainMenuOptions = new List<Menu.Option>();

    public GameTime GameTime;
    public Trigger Trigger;
    public Menu Menu;
    public MessageBox MessageBox;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public FoodItem WaterPrefab;
    public FoodItem BreadPrefab;
    public FoodItem MandarinPrefab;
    public FoodItem ApplePrefab;
    public FoodItem PotatoChipsPrefab;
    public FoodItem BiscuitsPrefab;
    public FoodItem ChocolateBarPrefab;
    public SleepItem SleepingBagPrefab;
    public PlayerSleepManager SleepManager;
    public InventoryItemDescription ItemDescription;
    public SleepHudButton SleepHudButton;


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
        
        mainMenuOptions.Add(new Menu.Option(OnFoodMenuSelected, "Buy food"));
        mainMenuOptions.Add(new Menu.Option(OnBuySleepingBag, "Buy a \"Pillow-Time\" sleeping bag"));
        mainMenuOptions.Add(new Menu.Option(OnExitSelected, "Exit"));
    }

    public void OnFoodMenuSelected()
    {
        List<Menu.Option> subMenuOptions = new List<Menu.Option>();
        subMenuOptions.Add(
            new Menu.Option(OnWaterSelected, "Water", WaterCost, PlayerState.CanAfford(WaterCost)));
        subMenuOptions.Add(
            new Menu.Option(OnBreadSelected, "Bread", BreadCost, PlayerState.CanAfford(BreadCost)));
        subMenuOptions.Add(
            new Menu.Option(OnMandarinSelected, "Mandarin", MandarinCost, PlayerState.CanAfford(MandarinCost)));
        subMenuOptions.Add(
            new Menu.Option(OnAppleSelected, "Apple", AppleCost, PlayerState.CanAfford(AppleCost)));
        subMenuOptions.Add(
            new Menu.Option(OnPotatoChipsSelected, "Potato Chips", PotatoChipsCost, PlayerState.CanAfford(PotatoChipsCost)));
        subMenuOptions.Add(
            new Menu.Option(OnBiscuitsSelected, "Biscuits", BiscuitsCost, PlayerState.CanAfford(BiscuitsCost)));
        subMenuOptions.Add(
            new Menu.Option(OnChocolateBarSelected, "Chocolate Bar", ChocolateBarCost, PlayerState.CanAfford(ChocolateBarCost)));
        subMenuOptions.Add(
            new Menu.Option(OnBackButtonSelected, "Back"));

        Menu.Show(subMenuOptions);
    }

    public void OnBackButtonSelected()
    {
        Menu.Show(mainMenuOptions);
    }

    public void OnWaterSelected()
    {
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= WaterCost)
        {
            PlayerState.Money -= WaterCost;

            // Add item.
            FoodItem item = Instantiate(WaterPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected();
    }

    public void OnBreadSelected()
    {
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= WaterCost)
        {
            PlayerState.Money -= BreadCost;

            // Add item.
            FoodItem item = Instantiate(BreadPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected(); 
    }

    public void OnMandarinSelected()
    {  
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= WaterCost)
        {
            PlayerState.Money -= MandarinCost;

            // Add item.
            FoodItem item = Instantiate(MandarinPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected();
    }

    public void OnAppleSelected()
    {
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= WaterCost)
        {
            PlayerState.Money -= AppleCost;

            // Add item.
            FoodItem item = Instantiate(ApplePrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected();
    }

    public void OnPotatoChipsSelected()
    {
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= WaterCost)
        {
            PlayerState.Money -= PotatoChipsCost;

            // Add item.
            FoodItem item = Instantiate(PotatoChipsPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected();
    }

    public void OnBiscuitsSelected()
    {
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= WaterCost)
        {
            PlayerState.Money -= BiscuitsCost;

            // Add item.
            FoodItem item = Instantiate(BiscuitsPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected();
    }

    public void OnChocolateBarSelected()
    {
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= WaterCost)
        {
            PlayerState.Money -= ChocolateBarCost;

            // Add item.
            FoodItem item = Instantiate(ChocolateBarPrefab);
            item.PlayerState = PlayerState;
            item.MessageBox = MessageBox;
            item.Inventory = Inventory;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected();
    }

    public void OnExitSelected()
    {
        reset();
    }


    public void OnTrigger()
    {
        Menu.Show(mainMenuOptions);
        Debug.Log("Yeah Boi");
    }

    public void OnBuySleepingBag()
    {
        if (!Inventory.IsInventoryFull() && PlayerState.Money >= SleepingBagCost)
        {
            PlayerState.Money -= SleepingBagCost;

            // Add item.
            SleepItem item = Instantiate(SleepingBagPrefab);
            item.SleepManager = SleepManager;
            item.ItemDescription = ItemDescription;
            item.SleepHudButton = SleepHudButton;
            Inventory.AddItem(item);
        }
        OnFoodMenuSelected();
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
