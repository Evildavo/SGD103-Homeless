using UnityEngine;
using System.Collections.Generic;

public class Dumpster : MonoBehaviour {
    float hourAtLastSearch;
    bool isPlayerSearching;

    // Represents the chance that a particular item is found.
    [System.Serializable]
    public class ItemProbability
    {
        public float Chance;
        public InventoryItem Item;
    }

    public Main Main;
    public Trigger Trigger;

    public float SearchTimeHours;
    public float BaseChanceSomethingFound;
    [Header("Note: Chances are automatically normalized (so that they add to 1).")]
    public List<ItemProbability> ChanceItemsFound;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(Reset);

        // Sort the item probabilities.
        ChanceItemsFound.Sort((a, b) => a.Chance.CompareTo(b.Chance));

        // Normalize probabilities.
        float totalProbability = 0.0f;
        foreach (ItemProbability itemProbability in ChanceItemsFound)
        {
            totalProbability += itemProbability.Chance;
        }
        foreach (ItemProbability itemProbability in ChanceItemsFound)
        {
            itemProbability.Chance = itemProbability.Chance / totalProbability;
        }
    }

    public void OnTrigger()
    {
        isPlayerSearching = true;
        hourAtLastSearch = Main.GameTime.TimeOfDayHours;
        Main.GameTime.AccelerateTime();
        Main.MessageBox.Show("Searching for food...", gameObject);
    }

    public void OnTriggerUpdate()
    {
        // Stop searching after some time.
        if (isPlayerSearching && 
            GameTime.TimeOfDayHoursDelta(hourAtLastSearch, Main.GameTime.TimeOfDayHours).forward > SearchTimeHours)
        {
            Main.GameTime.ResetToNormalTime();
            isPlayerSearching = false;
            

            // Determine if we found food
            if (Random.Range(0.0f, 1.0f) < BaseChanceSomethingFound && ChanceItemsFound.Count > 0)
            {
                // Determine what we found.
                FoodItem itemFound = null;
                float value = Random.Range(0.0f, 1.0f);
                float accumulatedChance = 0.0f;
                foreach (ItemProbability itemProbability in ChanceItemsFound)
                {
                    if (value < itemProbability.Chance + accumulatedChance)
                    {
                        // We found an item. Add it to the inventory.
                        itemFound = itemProbability.Item as FoodItem;
                        if (itemFound && !Main.Inventory.IsInventoryFull)
                        {
                            // Add item.
                            FoodItem item = Instantiate(itemFound);
                            item.Main = Main;
                            Main.Inventory.AddItem(item);

                            // Show message that we found the item.
                            Main.MessageBox.ShowForTime(
                                "You found a " + itemFound.ItemName, 3.0f, gameObject);
                        }
                        else
                        {
                            // If the inventory is full ask the player if they want to eat the food immediately.
                            ConfirmationBox.OnChoiceMade onChoiceMade = (bool yes) =>
                            {
                                if (yes)
                                {
                                    // Instantiate the item then eat it.
                                    FoodItem item = Instantiate(itemFound);
                                    item.Main = Main;
                                    item.OnPrimaryAction();
                                    Destroy(item);
                                }
                            };
                            Main.ConfirmationBox.Open(onChoiceMade, 
                                "You found a " + itemFound.ItemName + itemFound.MakeSubDescription() + 
                                ". Eat it?", "Yes", "No");
                            Main.MessageBox.ShowNext();
                        }
                        break;
                    }
                    accumulatedChance += itemProbability.Chance;
                }
                if (itemFound == null)
                {
                    Debug.LogWarning("Warning, failed to select the item found at a dumpster.");
                }
            }
            else
            {
                Main.MessageBox.ShowForTime("You didn't find anything", 3.0f, gameObject);
            }
            Reset();
        }
    }

    public void Reset()
    {
        isPlayerSearching = false;
        Main.GameTime.ResetToNormalTime();
        Trigger.Reset();
    }
    
}
