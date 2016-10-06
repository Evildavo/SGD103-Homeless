using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuTriggerTest : Trigger {

    public Menu Menu;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public MessageBox MessageBox;
    public FoodItemTest FoodItemPrefab;

    void OnOptionSelected(Menu.Option option)
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
                FoodItemTest foodItem = Instantiate(FoodItemPrefab);
                foodItem.PlayerState = PlayerState;
                foodItem.MessageBox = MessageBox;
                foodItem.Inventory = Inventory;
                foodItem.GetComponent<Image>().color = Random.ColorHSV(0.0f, 0.5f, 0.7f, 1.0f, 0.7f, 1.0f, 1.0f, 1.0f);
                Inventory.AddItem(foodItem);
                
                Debug.Log("You bought food");
                break;
            case "Alcohol":
                PlayerState.Money -= option.Price;
                if (PlayerState.Money < 0)
                {
                    PlayerState.Money = 0;
                }
                Debug.Log("You bought booze");
                break;
            case "A":
                MessageBox.ShowForTime(3, gameObject);
                MessageBox.SetMessage("Hello");

                Debug.Log("You said A");
                break;
            case "B":
                MessageBox.ShowForTime(3, gameObject);
                MessageBox.SetMessage("Cool");

                Debug.Log("You said B");
                break;
        }
    }

    public override void OnTrigger()
    {
        Menu.Option[] options = {
            new Menu.Option("Food", "Buy food", 10),
            new Menu.Option("Alcohol", "Buy alcohol", 200),
            new Menu.Option("A", "Say A"),
            new Menu.Option("B", "Say B")
        };
        Menu.SetOptions(options, OnOptionSelected);
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
