using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuTriggerTest : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;    
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
            OnBuyAlcoholSelected, "Buy alcohol", AlcoholPrice, Main.PlayerState.CanAfford(AlcoholPrice)));
        mainMenu.Add(new Menu.Option(OnOptionASelected, "Say A"));
        mainMenu.Add(new Menu.Option(OnOptionBSelected, "Say B"));
        if (Main.Inventory.HasItem(WatchItem))
        {
            mainMenu.Add(new Menu.Option(OnSellWatchSelected, "Sell watch", WatchPrice));
        }
        Main.Menu.Show(mainMenu);
    }

    public void OnFoodMenuSelected()
    {
        List<Menu.Option> options = new List<Menu.Option>();        
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy banana", 0.5f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy oats", 1.0f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy bread", 2.5f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy can beans", 4.0f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy biscuits", 5.0f));
        Main.Menu.Show(options);
    }
    
    public void OnBuyFoodSelected()
    {
        if (Main.PlayerState.Money >= FoodPrice && !Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= FoodPrice;

            // Add food to the inventory.
            FoodItemTest foodItem = Instantiate(FoodItem);
            foodItem.Main = Main;
            foodItem.GetComponent<Image>().color = Random.ColorHSV(0.0f, 0.5f, 0.7f, 1.0f, 0.7f, 1.0f, 1.0f, 1.0f);
            Main.Inventory.AddItem(foodItem);
        }
    }

    public void OnBuyAlcoholSelected()
    {
        if (Main.PlayerState.Money >= AlcoholPrice)
        {
            Main.PlayerState.Money -= AlcoholPrice;
        }
        ShowMainMenu();
    }

    public void OnOptionASelected()
    {
        Main.MessageBox.ShowForTime("Hello", null, gameObject);
        Main.Menu.Hide();
        Trigger.Reset();
    }

    public void OnOptionBSelected()
    {
        Main.MessageBox.ShowForTime("Cool", null, gameObject);
        Main.Menu.Hide();
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
                    Main.PlayerState.Money += WatchPrice;
                    Main.Inventory.RemoveItem(WatchItem);
                }
                Main.Menu.Hide();
                Trigger.Reset();
            };
            Main.ConfirmationBox.Open(onChoiceMade, "Sell watch?", "Yes", "No");
        }
        ShowMainMenu();
    }

    public void OnExit()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }

    public void OnTrigger()
    {
        ShowMainMenu();
    }
    
    public void OnPlayerExit()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }
        
}
