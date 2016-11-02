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

            Debug.Log("Stopping search");

            // Determine if we found food
            if (Random.Range(0.0f, 1.0f) < BaseChanceSomethingFound && ChanceItemsFound.Count > 0)
            {
                Debug.Log("Found something");

                // Determine what we found.
                InventoryItem itemFound = null;
                float value = Random.Range(0.0f, 1.0f);
                float accumulatedChance = 0.0f;
                foreach (ItemProbability itemProbability in ChanceItemsFound)
                {
                    Debug.Log(itemProbability.Item.ItemName + ": " + value + "/" + itemProbability.Chance + accumulatedChance);
                    if (value < itemProbability.Chance + accumulatedChance)
                    {
                        itemFound = itemProbability.Item;
                        Debug.Log(name + ": " + "Found a " + itemFound);
                        Main.MessageBox.ShowForTime(
                            "You found a " + itemProbability.Item.ItemName, 3.0f, gameObject);
                        break;
                    }
                    accumulatedChance += itemProbability.Chance;
                }

                if (itemFound == null)
                {
                    Debug.Log("Warning, didn't find anything");
                }
            }
            else
            {
                Debug.Log("Didn't find anything");
                Main.MessageBox.ShowForTime("You didn't find anything", 3.0f, gameObject);
            }
            Reset();
        }


        /*
        // Every second, randomly decide if we've found food.
        if (Time.time - timeAtLastCheck > 1.0f)
        {
            float value = Random.Range(0.0f, 1.0f);
            if (value <= ChanceOfFindingFoodPerSecond)
            {
                ConfirmationBox.OnChoiceMade onChoiceMade = (bool yes) =>
                {
                    if (yes)
                    {
                        Main.PlayerState.ChangeNutrition(NutritionBenefit);
                        Main.PlayerState.ChangeHealthTiredness(-HealthDetriment);
                    }
                };
                Main.ConfirmationBox.Open(onChoiceMade, "You found food. Eat it?", "Yes", "No");
                Main.GameTime.ResetToNormalTime();
                Trigger.Reset(false);
                // TODO: Reset later.
            }
            timeAtLastCheck = Time.time;
        }*/
    }

    public void Reset()
    {
        isPlayerSearching = false;
        Main.GameTime.ResetToNormalTime();
        //Main.ConfirmationBox.Close();
        Trigger.Reset();
    }
    
}
