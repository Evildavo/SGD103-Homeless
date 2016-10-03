using UnityEngine;
using System.Collections;

public class ObjectTriggerTest : Trigger {

    public AudioSource InteractSound;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public FoodItemTest FoodItemPrefab;
    public MessageBox MessageBox;

    public override void OnTrigger()
    {
        PlayerState.Money = 0.0f;
        if (Inventory && FoodItemPrefab)
        {
            FoodItemTest foodItem = Instantiate(FoodItemPrefab);
            foodItem.PlayerState = PlayerState;
            foodItem.MessageBox = MessageBox;
            Inventory.AddItem(foodItem);
        }
        else
        {
            PlayerState.HungerThirst = 1.0f;
        }
        if (InteractSound)
        {
            InteractSound.Play();
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

    public override void OnGUI()
    {
        base.OnGUI();
    }
}
