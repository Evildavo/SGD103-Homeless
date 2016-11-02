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
        [UnityEngine.Serialization.FormerlySerializedAs("Item")]
        public InventoryItem ItemPrefab;
    }

    public Main Main;
    public Trigger Trigger;

    public float MoralePenaltyForSearch;
    public float SearchTimeHours;
    public float InitialChanceSomethingFound;
    public float MinChanceSomethingFound;
    public float MaxChanceSomethingFound;
    [Header("Note: Chances are automatically normalized (so that they add to 1).")]
    public List<ItemProbability> ChanceItemsFound;
    public float[] BestTimesToSearch;
    public float ChanceDecreasePerHour;
    public float ChanceDecreaseWhenItemFound;
    public float MaxTimeToExpiryFound;
    public float MaxTimeAfterExpiryFound;
    [Header("Expiry bias decreases the chance food will be expired by the given percent")]
    public float InitialExpiryBias;
    public float MaxExpiryBias = 1.0f;
    public float ExpiryBiasDecreasePerHour;

    [ReadOnly]
    public float CurrentChance;
    [ReadOnly]
    public float CurrentExpiryBias;

    void Start()
    {
        CurrentChance = InitialChanceSomethingFound;
        CurrentExpiryBias = InitialExpiryBias;

        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(OnExit);

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

        // Apply a morale penalty for searching the garbage.
        Main.PlayerState.ChangeMorale(-MoralePenaltyForSearch);
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
                        itemFound = itemProbability.ItemPrefab as FoodItem;
                        if (itemFound)
                        {
                            if (!Main.Inventory.IsInventoryFull)
                            {
                                // Add item.
                                FoodItem item = Instantiate(itemFound);
                                item.Main = Main;
                                item.UpdateFoodExpiryCategory(makeFoodExpiry());
                                Main.Inventory.AddItem(item);

                                // Show message that we found the item.
                                Main.MessageBox.ShowForTime(
                                    "You found a " + item.ItemName + item.MakeSubDescription(), 
                                    3.0f, gameObject);
                                Reset();
                            }
                            else
                            {
                                // Instantiate the item.
                                FoodItem item = Instantiate(itemFound);
                                item.Main = Main;
                                item.UpdateFoodExpiryCategory(makeFoodExpiry());

                                // If the inventory is full ask the player if they want to eat the food immediately.
                                ConfirmationBox.OnChoiceMade onChoiceMade = (bool yes) =>
                                {
                                    if (yes)
                                    {
                                        // Eat the item.
                                        item.OnPrimaryAction();
                                        Destroy(item);
                                    }
                                    Reset();
                                };

                                Main.ConfirmationBox.Open(onChoiceMade,
                                    "You found a " + item.ItemName + item.MakeSubDescription() +
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

    public void OnExit()
    {
        Main.MessageBox.ShowNext();
        Reset();
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

        // Decrease the expiry bias over game-time. 
        if (CurrentExpiryBias > 0.0f)
        {
            CurrentExpiryBias -= ExpiryBiasDecreasePerHour * Main.GameTime.GameTimeDelta;
            if (CurrentExpiryBias < 0.0f)
            {
                CurrentExpiryBias = 0.0f;
            }
        }

        // At good times to search maximise the chance of finding an item and its expiry time.
        foreach (float hour in BestTimesToSearch)
        {
            if (GameTime.TimeOfDayHoursDelta(Main.GameTime.TimeOfDayHours, hour).shortest <= 
                Main.GameTime.GameTimeDelta)
            {
                CurrentChance = MaxChanceSomethingFound;
                CurrentExpiryBias = MaxExpiryBias;
                break;
            }
        }
    }
    
    // Generates a random food expiry biased based on the current chance of finding an item.
    float makeFoodExpiry()
    {
        return Random.Range(-MaxTimeAfterExpiryFound * (1.0f - CurrentExpiryBias), 
                            MaxTimeToExpiryFound);
    }

}
