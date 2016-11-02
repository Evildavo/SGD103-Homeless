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
    public float MinChanceSomethingFound;
    public float MaxChanceSomethingFound;
    [Header("Note: Chances are automatically normalized (so that they add to 1).")]
    public List<ItemProbability> ChanceItemsFound;
    public float[] BestTimesToSearch;
    public float ChanceDecreasePerHour;
    public float ChanceDecreaseWhenItemFound;

    [ReadOnly]
    public float CurrentChance;

    void Start()
    {
        CurrentChance = MinChanceSomethingFound;

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
            if (Random.Range(0.0f, 1.0f) < CurrentChance && ChanceItemsFound.Count > 0)
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
                        if (itemFound)
                        {
                            if (!Main.Inventory.IsInventoryFull)
                            {
                                // Add item.
                                FoodItem item = Instantiate(itemFound);
                                item.Main = Main;
                                Main.Inventory.AddItem(item);

                                // Show message that we found the item.
                                Main.MessageBox.ShowForTime(
                                    "You found a " + itemFound.ItemName, 3.0f, gameObject);
                                Reset();
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
                                    Reset();
                                };
                                Main.ConfirmationBox.Open(onChoiceMade,
                                    "You found a " + itemFound.ItemName + itemFound.MakeSubDescription() +
                                    ". Eat it?", "Yes", "No");
                                Main.MessageBox.Hide();
                            }

                            // Decrease the chance of finding an item again.
                            CurrentChance -= ChanceDecreaseWhenItemFound;
                            if (CurrentChance < MinChanceSomethingFound)
                            {
                                CurrentChance = MinChanceSomethingFound;
                            }
                        }
                        break;
                    }
                    accumulatedChance += itemProbability.Chance;
                }
                if (itemFound == null)
                {
                    Debug.LogWarning("Warning, failed to select the item found at a dumpster.");
                    Reset();
                }
            }
            else
            {
                Main.MessageBox.ShowForTime("You didn't find anything", 3.0f, gameObject);
                Reset();
            }
        }
    }

    public void Reset()
    {
        isPlayerSearching = false;
        Main.GameTime.ResetToNormalTime();
        Trigger.ResetWithCooloff();
    }

    void Update()
    {
        // Decrease the chance of an item being found over game-time.
        if (CurrentChance > MinChanceSomethingFound)
        {
            CurrentChance -= ChanceDecreasePerHour * Main.GameTime.GameTimeDelta;
            if (CurrentChance < MinChanceSomethingFound)
            {
                CurrentChance = MinChanceSomethingFound;
            }
        }

        // At good times to search maximise the chance of finding an item.
        foreach (float hour in BestTimesToSearch)
        {
            if (GameTime.TimeOfDayHoursDelta(Main.GameTime.TimeOfDayHours, hour).shortest <= 
                Main.GameTime.GameTimeDelta)
            {
                CurrentChance = MaxChanceSomethingFound;
                break;
            }
        }
    }
    
}
