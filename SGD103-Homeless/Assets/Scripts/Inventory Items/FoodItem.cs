using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FoodItem : MultiUseItem
{
    Color baseIconColour = Color.white;

    // The category of being expired.
    public enum ExpirationCategoryEnum
    {
        NOT,
        STALE,
        MOULDY,
        RANCID
    }

    [UnityEngine.Serialization.FormerlySerializedAs("HungerSatietyBenefitPerUse")]
    public float NutritionBenefitPerUse;
    public float TimeCostPerUse;

    [Header("Expiration settings.")]
    public float HoursToExpiry;
    public bool RandomiseInitialExpiry = true;
    public float NormallyLastsFromHours = 138.0f;
    public float NormallyLastsToHours = 168.0f;
    public float HoursAfterExpiryBeforeMouldy = 6.0f;
    public float HoursAfterExpiryBeforeRancid = 24.0f;
    public string StaleDescription = "stale";
    public string MouldyDescription = "mouldy";
    public string RancidDescription = "rancid";
    public float ChanceStaleHealthPenalty = 0.0f;
    public float ChanceMouldyHealthPenalty = 0.3f;
    public float ChanceRancidHealthPenalty = 0.8f;
    public float StaleHealthPenalty = 0.0f;
    public float MouldyHealthPenalty = 0.1f;
    public float RancidHealthPenalty = 0.25f;
    public float StaleNutritionFactor = 0.65f;
    public float MouldyNutritionFactor = 0.4f;
    public float RancidNutritionFactor = 0.2f;
    public Color StaleTextColour = Color.white;
    public Color MouldyTextColour = Color.white;
    public Color RancidTextColour = Color.white;
    [Header("These colours are multiplied with the base colour of the object")]
    public Color StaleIconColour = Color.white;
    public Color MouldyIconColour = Color.white;
    public Color RancidIconColour = Color.white;
    [ReadOnly]
    public ExpirationCategoryEnum ExpirationCategory;

    // Randomly generates an expiry time in the given range.
    // Negative numbers are considered past expiry.
    public void RandomiseExpiry(float fromHour, float toHour)
    {
        UpdateFoodExpiryCategory(Random.Range(fromHour, toHour));
    }

    public void UpdateFoodExpiryCategory(float hoursToExpiry)
    {
        HoursToExpiry = hoursToExpiry;
        if (hoursToExpiry >= 0.0f)
        {
            SubDescription = "";
            GetComponent<Image>().color = baseIconColour;
            ExpirationCategory = ExpirationCategoryEnum.NOT;
        }
        else if (hoursToExpiry > -HoursAfterExpiryBeforeMouldy)
        {
            SubDescription = StaleDescription;
            GetComponent<Image>().color = baseIconColour * StaleIconColour;
            ExpirationCategory = ExpirationCategoryEnum.STALE;
        }
        else if (hoursToExpiry > -HoursAfterExpiryBeforeRancid)
        {
            SubDescription = MouldyDescription;
            GetComponent<Image>().color = baseIconColour * MouldyIconColour;
            ExpirationCategory = ExpirationCategoryEnum.MOULDY;
        }
        else
        {
            SubDescription = RancidDescription;
            GetComponent<Image>().color = baseIconColour * RancidIconColour;
            ExpirationCategory = ExpirationCategoryEnum.RANCID;
        }
    }

    public override void OnPrimaryAction()
    {
        var PlayerState = Main.PlayerState;
        var MessageBox = Main.MessageBox;

        // Determine if the item makes the player sick for eating.
        bool madeSick = false;
        if (ExpirationCategory == ExpirationCategoryEnum.STALE &&
            Random.Range(0.0f, 1.0f) < ChanceStaleHealthPenalty)
        {
            madeSick = true;
            Main.PlayerCharacter.Vomit();
            Main.PlayerState.ChangeHealthTiredness(-StaleHealthPenalty);
        }
        else if (ExpirationCategory == ExpirationCategoryEnum.MOULDY &&
            Random.Range(0.0f, 1.0f) < ChanceMouldyHealthPenalty)
        {
            madeSick = true;
            Main.PlayerCharacter.Vomit();
            Main.PlayerState.ChangeHealthTiredness(-MouldyHealthPenalty);
        }
        else if (ExpirationCategory == ExpirationCategoryEnum.RANCID &&
            Random.Range(0.0f, 1.0f) < ChanceRancidHealthPenalty)
        {
            madeSick = true;
            Main.PlayerCharacter.Vomit();
            Main.PlayerState.ChangeHealthTiredness(-RancidHealthPenalty);
        }
        else
        {
            // Increase stat based on expiration category.
            if (ExpirationCategory == ExpirationCategoryEnum.NOT)
            {
                PlayerState.ChangeNutrition(NutritionBenefitPerUse);
            }
            else if (ExpirationCategory == ExpirationCategoryEnum.STALE)
            {
                PlayerState.ChangeNutrition(NutritionBenefitPerUse * StaleNutritionFactor);
            }
            else if (ExpirationCategory == ExpirationCategoryEnum.MOULDY)
            {
                PlayerState.ChangeNutrition(NutritionBenefitPerUse * MouldyNutritionFactor);
            }
            else if (ExpirationCategory == ExpirationCategoryEnum.RANCID)
            {
                PlayerState.ChangeNutrition(NutritionBenefitPerUse * RancidNutritionFactor);
            }
        }

        // Apply time cost.
        Main.GameTime.SpendTime(TimeCostPerUse);

        // Show message depending on how full the player is after eating.
        if (!madeSick)
        {
            if (PlayerState.Nutrition + NutritionBenefitPerUse > 0.6f)
            {
                MessageBox.ShowForTime("You feel full", null, gameObject);
            }
            else if (PlayerState.Nutrition + NutritionBenefitPerUse > 0.4f)
            {
                MessageBox.ShowForTime("You needed that", null, gameObject);
            }
            else
            {
                MessageBox.ShowForTime("You're still hungry", null, gameObject);
            }
        }
        else
        {
            MessageBox.ShowForTime("That food made you feel sick", null, gameObject);
        }

        // Consume item.
        NumUses -= 1;
        if (NumUses == 0)
        {
            Main.Inventory.RemoveItem(this);
        }
    }

    // Call from derived.
    public override void OnCursorEnter()
    {
        if (ExpirationCategory == ExpirationCategoryEnum.NOT)
        {
            Main.ItemDescription.ItemName.color = Color.white;
        }
        else if (ExpirationCategory == ExpirationCategoryEnum.STALE)
        {
            Main.ItemDescription.ItemName.color = StaleTextColour;
        }
        else if (ExpirationCategory == ExpirationCategoryEnum.MOULDY)
        {
            Main.ItemDescription.ItemName.color = MouldyTextColour;
        }
        else if (ExpirationCategory == ExpirationCategoryEnum.RANCID)
        {
            Main.ItemDescription.ItemName.color = RancidTextColour;
        }
    }

    // Call from derived.
    public override void OnCursorExit()
    {
        Main.ItemDescription.ItemName.color = Color.white;
    }

    public void UpdateBaseColour()
    {
        baseIconColour = GetComponent<Image>().color;
    }

    // Call from derived.
    protected new void Start()
    {
        base.Start();
        UpdateBaseColour();
        if (RandomiseInitialExpiry)
        {
            RandomiseExpiry(NormallyLastsFromHours, NormallyLastsToHours);
        }
    }

    // Call from derived.
    protected void Update()
    {
        UpdateItemDescription();

        // Increment expiry.
        HoursToExpiry -= Main.GameTime.GameTimeDelta;

        // Update expiration category.
        UpdateFoodExpiryCategory(HoursToExpiry);
    }

}