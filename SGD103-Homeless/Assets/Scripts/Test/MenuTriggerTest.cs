using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuTriggerTest : Trigger {

    public Menu Menu;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public FoodItemTest FoodItem;
    public WatchItemTest WatchItem;
    
    
    public List<Menu.Option> Options;
    
    public void OnBuyFoodSelected(string name, int price)
    {
        PlayerState.Money += price;
        if (PlayerState.Money < 0)
        {
            PlayerState.Money = 0;
        }

        // Add food to the inventory.
        FoodItemTest foodItem = Instantiate(FoodItem);
        foodItem.PlayerState = PlayerState;
        foodItem.MessageBox = MessageBox;
        foodItem.Inventory = Inventory;
        foodItem.GetComponent<Image>().color = Random.ColorHSV(0.0f, 0.5f, 0.7f, 1.0f, 0.7f, 1.0f, 1.0f, 1.0f);
        Inventory.AddItem(foodItem);
    }

    public void OnBuyAlcoholSelected(string name, int price)
    {
        PlayerState.Money += price;
        if (PlayerState.Money < 0)
        {
            PlayerState.Money = 0;
        }
    }

    public void OnOptionASelected(string name, int price)
    {
        MessageBox.ShowForTime(3, gameObject);
        MessageBox.SetMessage("Hello");
    }

    public void OnOptionBSelected(string name, int price)
    {
        MessageBox.ShowForTime(3, gameObject);
        MessageBox.SetMessage("Cool");
    }

    public void OnSellWatchSelected(string name, int price)
    {
        if (WatchItem)
        {
            // Anonymous function for when the user presses confirm in the confirmation box.
            ConfirmationBox.OnConfirmation onSellWatchConfirmed = () =>
            {
                PlayerState.Money += price;
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
    
    public override void OnTrigger()
    {
        Menu.Show(Options);
        IsActive = true;
    }

    public override void OnPlayerEnter()
    {
    }

    public override void OnPlayerExit()
    {
    }


    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
