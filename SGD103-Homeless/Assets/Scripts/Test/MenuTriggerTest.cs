using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuTriggerTest : MonoBehaviour {
    
    public Menu Menu;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public FoodItemTest FoodItem;
    public WatchItemTest WatchItem;    
    public List<Menu.Option> Options;
    
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
    }

    public void OnOptionBSelected(string name, int value)
    {
        MessageBox.ShowForTime(3, gameObject);
        MessageBox.SetMessage("Cool");
        Menu.Hide();
    }

    public void OnSellWatchSelected(string name, int value)
    {
        if (WatchItem)
        {
            // Anonymous function for when the user presses confirm in the confirmation box.
            ConfirmationBox.OnConfirmation onSellWatchConfirmed = () =>
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
                Menu.Show(Options);
            };
            ConfirmationBox.Open(onSellWatchConfirmed, "Sell watch?", "Yes", "No");
        }
    }
    
    public void OnTrigger(Trigger trigger)
    {
        Debug.Log("Triggered");
        Menu.Show(Options);
        trigger.Reset();
    }
    
    public void OnPlayerExit(Trigger trigger)
    {
        Menu.Hide();
    }
        
}
