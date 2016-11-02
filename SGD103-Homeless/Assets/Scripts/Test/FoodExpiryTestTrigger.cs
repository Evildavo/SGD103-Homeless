using UnityEngine;
using System.Collections.Generic;

public class FoodExpiryTestTrigger : MonoBehaviour {

    public Main Main;
    public Trigger Trigger;
    public FoodItem FoodPrefab;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OpenMainMenu);
        Trigger.RegisterOnCloseRequested(Reset);
    }
    
    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(GoodFoodSelected, "Good food"));
        options.Add(new Menu.Option(StaleFoodSelected, "Stale food"));
        options.Add(new Menu.Option(MouldyFoodSelected, "Mouldy food"));
        options.Add(new Menu.Option(RancidFoodSelected, "Rancid food"));
        options.Add(new Menu.Option(Reset, "Exit"));

        Main.Menu.Show(options);
    }

    public void GoodFoodSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            FoodItem item = Instantiate(FoodPrefab);
            item.Main = Main;
            item.HoursToExpiry = 24.0f;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenMainMenu();
    }

    public void StaleFoodSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            FoodItem item = Instantiate(FoodPrefab);
            item.Main = Main;
            item.HoursToExpiry = -2.0f;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenMainMenu();
    }

    public void MouldyFoodSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            FoodItem item = Instantiate(FoodPrefab);
            item.Main = Main;
            item.HoursToExpiry = -12.0f;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenMainMenu();
    }

    public void RancidFoodSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            FoodItem item = Instantiate(FoodPrefab);
            item.Main = Main;
            item.HoursToExpiry = -36.0f;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        OpenMainMenu();
    }

    void Reset()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }

}
