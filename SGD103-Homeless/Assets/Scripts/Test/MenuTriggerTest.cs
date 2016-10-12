using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuTriggerTest : MonoBehaviour {

    public Trigger Trigger;    
    public Menu Menu;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public FoodItemTest FoodItem;
    public WatchItemTest WatchItem;    
    public List<Menu.Option> Options;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
    }
    
    public void OnBuyFoodSelected(string name, int value)
    {
        if (PlayerState.Money >= value && !Inventory.IsInventoryFull())
        {
            PlayerState.Money -= value;

            // Add food to the inventory.
            FoodItemTest foodItem = Instantiate(FoodItem);
            foodItem.PlayerState = PlayerState;
            foodItem.MessageBox = MessageBox;
            foodItem.Inventory = Inventory;
            foodItem.GetComponent<Image>().color = Random.ColorHSV(0.0f, 0.5f, 0.7f, 1.0f, 0.7f, 1.0f, 1.0f, 1.0f);
            Inventory.AddItem(foodItem);
        }
    }

    public void OnBuyAlcoholSelected(string name, int value)
    {
        if (PlayerState.Money >= value)
        {
            PlayerState.Money -= value;
        }
    }

    public void OnOptionASelected(string name, int value)
    {
        MessageBox.ShowForTime(3, gameObject);
        MessageBox.SetMessage("Hello");
        Menu.Hide();
        Trigger.Reset();
    }

    public void OnOptionBSelected(string name, int value)
    {
        MessageBox.ShowForTime(3, gameObject);
        MessageBox.SetMessage("Cool");
        Menu.Hide();
        Trigger.Reset();
    }

    public void OnSellWatchSelected(string name, int value)
    {
        if (WatchItem)
        {
            ConfirmationBox.OnChoiceMade onChoiceMade = (bool yes) =>
            {
                if (yes)
                {
                    PlayerState.Money += value;
                    Inventory.RemoveItem(WatchItem);

                    // Remove this option and update the menu.
                    for (var i = 0; i < Options.Count; i++)
                    {
                        if (Options[i].Name == name)
                        {
                            Options.RemoveAt(i);
                        }
                    }
                }
                Menu.Hide();
                Trigger.Reset();
            };
            ConfirmationBox.Open(onChoiceMade, "Sell watch?", "Yes", "No");
        }
    }
    
    public void OnTrigger()
    {
        Menu.Show(Options);
    }
    
    public void OnPlayerExit()
    {
        Menu.Hide();
        Trigger.Reset();
    }
        
}
