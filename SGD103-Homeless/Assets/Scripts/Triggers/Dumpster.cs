using UnityEngine;
using System.Collections.Generic;

public class Dumpster : MonoBehaviour {
    float hourAtLastSearch;
    bool isPlayerSearching;
    float chanceLostFromItemsFound;
    public float randomExpiryFrom;
    public float randomExpiryTo;

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
    [UnityEngine.Serialization.FormerlySerializedAs("BestTimesToSearch")]
    public float[] GoodHoursToSearchFrom;
    public float BestChanceSomethingFound;
    public float WorstChanceSomethingFound;
    public float TimeFromGoodHourBeforeChanceIsWorst;
    public float TimeFromGoodHourBeforeExpiryIsWorst;
    public float ChanceDecreaseWhenItemFound;
    [Header("Note: Chances are automatically normalized (so that they add to 1).")]
    public List<ItemProbability> ChanceItemsFound;
    public float WorstExpiryFrom;
    public float WorstExpiryTo;
    public float BestExpiryFrom;
    public float BestExpiryTo;

    [Space(10.0f)]
    [ReadOnly]
    public float CurrentChance;
    [ReadOnly]
    public float CurrentExpiryBias;

    void updateChance()
    {
        // Find the distance to the most recent good hour to search from.
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < GoodHoursToSearchFrom.Length; i++)
        {
            float delta = GameTime.TimeOfDayHoursDelta(Main.GameTime.TimeOfDayHours,
                                                       GoodHoursToSearchFrom[i]).backward;
            if (delta < nearestDistance)
            {
                nearestDistance = delta;
            }
        }

        // Calculate chance based on the distance to the most recent good hour to search.
        CurrentChance = Mathf.Max(
            WorstChanceSomethingFound +
            (BestChanceSomethingFound - WorstChanceSomethingFound) *
            (1.0f - nearestDistance / TimeFromGoodHourBeforeChanceIsWorst) -
            chanceLostFromItemsFound,
            WorstChanceSomethingFound);

        // Calculate expiry factor based on the distance to the most recent good hour to search.
        CurrentExpiryBias = Mathf.Max(
            1.0f - nearestDistance / TimeFromGoodHourBeforeExpiryIsWorst,
            0.0f);
    }

    void Start()
    {
        updateChance();

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
                                item.RandomiseInitialExpiry = false;
                                item.UpdateBaseColour();
                                updateExpiryRandomRange();
                                item.RandomiseExpiry(randomExpiryFrom, randomExpiryTo);
                                Main.Inventory.AddItem(item);

                                // Show message that we found the item.
                                Main.MessageBox.ShowForTime(
                                    "You found a " + item.ItemName + item.MakeSubDescription(),
                                    null, gameObject);
                                Reset();
                            }
                            else
                            {
                                // Instantiate the item.
                                FoodItem item = Instantiate(itemFound);
                                item.Main = Main;
                                item.RandomiseInitialExpiry = false;
                                item.UpdateBaseColour();
                                updateExpiryRandomRange();
                                item.RandomiseExpiry(randomExpiryFrom, randomExpiryTo);

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
                            chanceLostFromItemsFound += ChanceDecreaseWhenItemFound;
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
                Main.MessageBox.ShowForTime("You didn't find anything", null, gameObject);
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

    void updateExpiryRandomRange()
    {
        randomExpiryFrom = WorstExpiryFrom * (1.0f - CurrentExpiryBias) + BestExpiryFrom * CurrentExpiryBias;
        randomExpiryTo = WorstExpiryTo * (1.0f - CurrentExpiryBias) + BestExpiryTo * CurrentExpiryBias;
    }

    void Update()
    {
        // At good times to search maximise reset the chance decrease from finding items.
        foreach (float hour in GoodHoursToSearchFrom)
        {
            if (GameTime.TimeOfDayHoursDelta(Main.GameTime.TimeOfDayHours, hour).shortest <= 
                Main.GameTime.GameTimeDelta)
            {
                chanceLostFromItemsFound = 0.0f;
                break;
            }
        }

        // Update chance an item is found and the expiry factor.
        updateChance();
    }

}
