using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectTriggerTest : MonoBehaviour {

    public Main Main;
    public Trigger Trigger;
    public AudioSource InteractSound;
    public FoodItemTest FoodItemPrefab;
    public bool FoodIsFree = false;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
    }

    public void OnTrigger()
    {
        // Buy the item if we have room in the inventory.
        var Inventory = Main.Inventory;
        var PlayerState = Main.PlayerState;
        if (!Inventory.IsInventoryFull)
        {
            // Charge the player everything they have.
            if (!FoodIsFree)
            {
                PlayerState.Money = 0.0f;
            }

            // Add food to the inventory.
            if (Inventory && FoodItemPrefab)
            {
                FoodItemTest foodItem = Instantiate(FoodItemPrefab);
                foodItem.Main = Main;
                foodItem.GetComponent<Image>().color = Random.ColorHSV(0.0f, 0.5f, 0.7f, 1.0f, 0.7f, 1.0f, 1.0f, 1.0f);
                Inventory.AddItem(foodItem);
            }
            // Eat immediately if we're not adding to the inventory.
            else
            {
                PlayerState.Nutrition = 1.0f;
            }

            // Play sound.
            if (InteractSound)
            {
                InteractSound.Play();
            }
        }
        
        // Reset trigger so the player can buy more.
        if (FoodIsFree)
        {
            Trigger.Reset();
        }
        else
        {
            Trigger.Reset(false);
        }
    }

}
