using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuTriggerTest : Trigger {

    public Menu Menu;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public MessageBox MessageBox;
    public ConfirmationBox ConfirmationBox;
    public FoodItemTest FoodItem;
    public WatchItemTest WatchItem;
    
    void onOptionSelected(Menu.Option option)
    {
        switch (option.ID)
        {
            case "Food":
                PlayerState.Money -= option.Price;
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
                break;
            case "Alcohol":
                PlayerState.Money -= option.Price;
                if (PlayerState.Money < 0)
                {
                    PlayerState.Money = 0;
                }
                break;
            case "A":
                MessageBox.ShowForTime(3, gameObject);
                MessageBox.SetMessage("Hello");
                break;
            case "B":
                MessageBox.ShowForTime(3, gameObject);
                MessageBox.SetMessage("Cool");
                break;
            case "Sell":

                // Anonymous function for when the user presses confirm in the confirmation box.
                ConfirmationBox.OnConfirmation onSellWatchConfirmed = () =>
                {
                    PlayerState.Money += option.Price;
                    Inventory.RemoveItem(WatchItem);
                };
                ConfirmationBox.Open(onSellWatchConfirmed, "Sell watch?", "Yes", "No");
                break;
        }
    }
    
    public override void OnTrigger()
    {
        Menu.Option[] options = {
            new Menu.Option("Food", "Buy food", -10),
            new Menu.Option("Alcohol", "Buy alcohol", -200),
            new Menu.Option("A", "Say A"),
            new Menu.Option("B", "Say B"),
            new Menu.Option("Sell", "Sell watch", 3000)
        };
        Menu.SetOptions(options, onOptionSelected);
        IsActive = true;
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
