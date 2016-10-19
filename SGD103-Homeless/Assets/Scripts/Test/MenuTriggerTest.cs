using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuTriggerTest : MonoBehaviour
{
    public Trigger Trigger;    
    public Menu Menu;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public FoodItemTest FoodItem;
    public WatchItem WatchItem;

    public float FoodPrice = 10.0f;
    public float AlcoholPrice = 30.0f;
    public float WatchPrice = 300.0f;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }

    public void ShowMainMenu()
    {
        List<Menu.Option> mainMenu = new List<Menu.Option>();
        mainMenu.Add(new Menu.Option(OnFoodMenuSelected, "Buy food"));
        mainMenu.Add(new Menu.Option(
            OnBuyAlcoholSelected, "Buy alcohol", AlcoholPrice, PlayerState.CanAfford(AlcoholPrice)));
        mainMenu.Add(new Menu.Option(OnOptionASelected, "Say A"));
        mainMenu.Add(new Menu.Option(OnOptionBSelected, "Say B"));
        if (Inventory.HasItem(WatchItem))
        {
            mainMenu.Add(new Menu.Option(OnSellWatchSelected, "Sell watch", WatchPrice));
        }
        Menu.Show(mainMenu);
    }

    public void OnFoodMenuSelected()
    {
        List<Menu.Option> options = new List<Menu.Option>();        
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy banana", 0.5f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy oats", 1.0f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy bread", 2.5f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy can beans", 4.0f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy biscuits", 5.0f));
        Menu.Show(options);
    }
    
    public void OnBuyFoodSelected()
    {
        if (PlayerState.Money >= FoodPrice && !Inventory.IsInventoryFull())
        {
            PlayerState.Money -= FoodPrice;

            // Add food to the inventory.
            FoodItemTest foodItem = Instantiate(FoodItem);
            foodItem.PlayerState = PlayerState;
            foodItem.MessageBox = MessageBox;
            foodItem.Inventory = Inventory;
            foodItem.GetComponent<Image>().color = Random.ColorHSV(0.0f, 0.5f, 0.7f, 1.0f, 0.7f, 1.0f, 1.0f, 1.0f);
            Inventory.AddItem(foodItem);
        }
    }

    public void OnBuyAlcoholSelected()
    {
        if (PlayerState.Money >= AlcoholPrice)
        {
            PlayerState.Money -= AlcoholPrice;
        }
        ShowMainMenu();
    }

    public void OnOptionASelected()
    {
        MessageBox.ShowForTime("Hello", 3, gameObject);
        Menu.Hide();
        Trigger.Reset();
    }

    public void OnOptionBSelected()
    {
        MessageBox.ShowForTime("Cool", 3, gameObject);
        Menu.Hide();
        Trigger.Reset();
    }

    public void OnSellWatchSelected()
    {
        if (WatchItem)
        {
            // In-line anonymous function.
            ConfirmationBox.OnChoiceMade onChoiceMade = (bool yes) =>
            {
                if (yes)
                {
                    PlayerState.Money += WatchPrice;
                    Inventory.RemoveItem(WatchItem);
                }
                Menu.Hide();
                Trigger.Reset();
            };
            ConfirmationBox.Open(onChoiceMade, "Sell watch?", "Yes", "No");
        }
        ShowMainMenu();
    }

    public void OnExit()
    {
        Menu.Hide();
        Trigger.Reset();
    }

    public void OnTrigger()
    {
        ShowMainMenu();
    }
    
    public void OnPlayerExit()
    {
        Menu.Hide();
        Trigger.Reset();
    }
        
}
