using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuTriggerTest : MonoBehaviour
{
    private List<Menu.Option> options = new List<Menu.Option>();

    public Trigger Trigger;    
    public Menu Menu;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public FoodItemTest FoodItem;
    public WatchItemTest WatchItem;

    public float FoodPrice = 10.0f;
    public float AlcoholPrice = 30.0f;
    public float WatchPrice = 300.0f;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);

        // Set up menu.
        options.Add(new Menu.Option(OnFoodMenuSelected, "Buy food"));
        options.Add(new Menu.Option(OnBuyAlcoholSelected, "Buy alcohol", AlcoholPrice));
        options.Add(new Menu.Option(OnOptionASelected, "Say A"));
        options.Add(new Menu.Option(OnOptionBSelected, "Say B"));
        options.Add(new Menu.Option(OnSellWatchSelected, "Sell watch", WatchPrice));
        options.Add(new Menu.Option(OnExit, "Exit"));
    }

    public void OnFoodMenuSelected()
    {
        Menu.Show(GetFoodSubMenu());
    }

    public List<Menu.Option> GetFoodSubMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy banana", 0.5f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy oats", 1.0f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy bread", 2.5f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy can beans", 4.0f));
        options.Add(new Menu.Option(OnBuyFoodSelected, "Buy biscuits", 5.0f));
        return options;
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

                    // Remove this option and update the menu.
                    for (var i = 0; i < options.Count; i++)
                    {
                        if (options[i].Name == name)
                        {
                            options.RemoveAt(i);
                        }
                    }
                }
                Menu.Hide();
                Trigger.Reset();
            };
            ConfirmationBox.Open(onChoiceMade, "Sell watch?", "Yes", "No");
        }
    }

    public void OnExit()
    {
        Menu.Hide();
        Trigger.Reset();
    }

    public void OnTrigger()
    {
        Menu.Show(options);
    }
    
    public void OnPlayerExit()
    {
        Menu.Hide();
        Trigger.Reset();
    }
        
}
