using UnityEngine;
using System.Collections.Generic;

public class Laundromat : MonoBehaviour
{ 
    public Main Main;
    public Trigger Trigger;

    public float CostToUse;
    public float TimeCostToUse;

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onWashClothes, "Wash and dry clothes", CostToUse, Main.PlayerState.CanAfford(CostToUse)));
        options.Add(new Menu.Option(OnExit, "Exit"));

        Main.Menu.Show(options);
    }
    
    public void onWashClothes()
    {
        // Pay money.
        Main.PlayerState.Money -= CostToUse;

        // Apply time cost.
        Main.GameTime.SpendTime(TimeCostToUse);

        // Wash all clothes in the player inventory.
        foreach (ClothingItem item in Main.Inventory.ItemContainer.GetComponentsInChildren<ClothingItem>())
        {
            item.Cleanliness = 1.0f;
        }
    }
    
    void Start ()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnCloseRequested(reset);
    }
    	
	void OnTrigger () {
        OpenMainMenu();
    }

    void OnExit()
    {
        reset();
        Main.MessageBox.ShowNext();
    }

    void reset()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }

}
