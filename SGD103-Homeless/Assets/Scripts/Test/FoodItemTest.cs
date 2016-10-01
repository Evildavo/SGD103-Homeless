using UnityEngine;
using System.Collections;

public class FoodItemTest : InventoryItem
{
    public PlayerState PlayerState;

    public override void OnPrimaryAction()
    {
        PlayerState.HungerThirst += 1.0f;
        Destroy(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }

}
