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
    public WaterItem WaterPrefab;

    public float WaterCost;
    

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        
        mainMenuOptions.Add(new Menu.Option(OnFoodMenuSelected, "Buy food"));
        mainMenuOptions.Add(new Menu.Option(OnExitSelected, "Exit"));
    }

    public void OnFoodMenuSelected()
    {
        List<Menu.Option> subMenuOptions = new List<Menu.Option>();
        subMenuOptions.Add(new Menu.Option(OnWaterSelected, "Water", WaterCost));
        subMenuOptions.Add(new Menu.Option(OnBackButtonSelected, "Back"));

        Menu.Show(subMenuOptions);
    }

    public void OnBackButtonSelected()
    {
        Menu.Show(mainMenuOptions);
    }

    public void OnWaterSelected()
    {  
        if (!Inventory.IsInventoryFull())
        {
            PlayerState.Money -= WaterCost;

            // Add item.
            WaterItem waterItem = Instantiate(WaterPrefab);
            waterItem.PlayerState = PlayerState;
            waterItem.MessageBox = MessageBox;
            waterItem.Inventory = Inventory;
            Inventory.AddItem(waterItem);
        }
    }

    public void OnBreadSelected()
    {
        PlayerState.Money -= 2f;
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
