using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectTriggerTest : Trigger {

    public AudioSource InteractSound;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public FoodItemTest FoodItemPrefab;
    public MessageBox MessageBox;
    public bool FoodIsFree = false;

    public override void OnTrigger()
    {
        // Buy the item if we have room in the inventory.
        if (!Inventory.IsInventoryFull())
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
                foodItem.PlayerState = PlayerState;
                foodItem.MessageBox = MessageBox;
                foodItem.GetComponent<Image>().color = Random.ColorHSV(0.0f, 0.5f, 0.7f, 1.0f, 0.7f, 1.0f, 1.0f, 1.0f);
                Inventory.AddItem(foodItem);
            }
            // Eat immediately if we're not adding to the inventory.
            else
            {
                PlayerState.HungerThirst = 1.0f;
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
            IsActive = true;
        }
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
