using UnityEngine;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour {
    float timeAtLastVomit;
    bool hasVomited = false;
    float timeAtLastObjectiveSpawn;
    bool hasSpawnedObjective = false;
    List<Objective> alcoholObjectives = new List<Objective>();
    bool hasStomachGrowled = false;
    float timeAtLastStomachGrowl;
    bool hasCoughed = false;
    float timeAtLastCough;
    float hoursSinceDepressionLastTreated;
    float moraleChangeRemaining;
    float nutritionChangeRemaining;
    float healthChangeRemaining;
    float highlightMoraleForSeconds;
    float highlightNutritionForSeconds;
    float highlightHealthForSeconds;
    bool highlightMoralePositive;
    bool highlightMoraleNegative;
    bool highlightNutritionPositive;
    bool highlightNutritionNegative;
    bool highlightHealthPositive;
    bool highlightHealthNegative;
    float timeAtMoraleHighlight;
    float timeAtNutritionHighlight;
    float timeAtHealthHighlight;

    public Main Main;

    [Header("Display settings:")]
    public Color NormalTextColour = Color.black;
    public Color WarningTextColour = Color.red;
    public Color HighlightTextColour = Color.yellow;
    public bool BoldTextDuringWarning = true;
    public float NutritionWarningThreshold = 0.2f;
    public float HealthWarningThreshold = 0.2f;
    public float MoraleWarningThreshold = 0.2f;

    [Header("Stats:")]
    public float Money = 0;
    [Range(0.0f, 1.0f)]
    public float Nutrition = 1.0f;
    [Range(0.0f, 1.0f)]
    public float HealthTiredness = 1.0f;
    [Range(0.0f, 1.0f)]
    public float Morale = 1.0f;

    [Space(10.0f)]
    [Range(0.0f, 1.0f)]
    public float Inebriation = 0.0f;
    [Range(0.0f, 1.0f)]
    public float Addiction = 0.0f;
    [Range(0.0f, 1.0f)]
    public float CurrentClothingCleanliness = 1.0f;
    public bool IsDepressionTreated = true;
    
    [Header("Settings:")] 
    public float NutritionLossPerHour = 0.0f;        
    public float HealthLossPerHour = 0.0f;
    public float MoraleLossPerHour = 0.0f;
    
    [Space(10.0f)]
    public float MaxNutritionHealthPenaltyPerHour = 0.0f;
    [Range(0.0f, 1.0f)]
    public float NutritionOkAtLevel = 0.0f;
    public float MaxNutritionHealthRewardPerHour = 0.0f;

    [Header("Nutrition goes down slower when in good condition")]
    public float MaxNutritionRewardPerHour = 0.0f;

    [Space(10.0f)]
    public float MaxPoorHealthMoralePenaltyPerHour = 0.0f;
    [Range(0.0f, 1.0f)]
    public float HealthGoodAtLevel = 0.0f;
    public float MaxGoodHealthMoraleRewardPerHour = 0.0f;

    [Space(10.0f)]
    public float SleepNutritionGainFactor = 0.0f;
    public float SleepMoraleLossFactor = 0.0f;
    [Header("Not counting bonus from a sleeping bag")]
    public float MinSleepingRoughHealthGainPerHour = 0.0f;
    public float MaxSleepingRoughHealthGainPerHour = 0.0f;

    [Space(10.0f)]
    public float WorkNutritionGainFactor = 1.0f;
    public float WorkHealthLossFactor = 1.0f;
    [Header("See Job Locations for morale gain settings.")]
    public float WorkMoraleLossFactor = 1.0f;

    [Space(10.0f)]
    public float InebriationDecreasesPerHour;
    public float AlcoholEffectsStartAtInebriation = 1.0f;
    [Header("Turns slightly while walking")]
    public bool WalkWonkeyWhenIntoxicated;
    public bool VomitWhenIntoxicated;
    public bool WalkSlowerWhenIntoxicated;
    public float WonkyWalkAngleAtMaxInebriationDegrees;
    public float VomitIntervalAtMaxInebriationSeconds;
    public float WalkSpeedFactorAtMaxInebriation = 1.0f;

    [Space(10.0f)]
    public float MaxAddictionMoralePenaltyPerHour;
    public float SpawnObjectivesAboveAddictionLevel;
    public float ObjectiveSpawnIntervalAtMaxAddictionSeconds;
    public float AddictionReducedPerMoraleGainedFactor = 1.0f;

    [Space(10.0f)]
    public float PoorHealthEffectsBelowLevel;
    public bool WalkSlowerWhenHealthIsPoor;
    public bool CoughWhenHealthIsPoor;
    public float WalkSpeedFactorAtWorstHealth;
    public float CoughIntervalAtWorstHealth;

    [Space(10.0f)]
    public float PoorMoraleEffectsBelowLevel;
    [Header("Walking slower is a symptom of depression in reality")]
    public bool WalkSlowerWhenMoraleIsPoor;
    public float WalkSpeedFactorAtLowestMorale;

    [Space(10.0f)]
    public float StomachGrowlBelowNutrition;
    public float StomachGrowlIntervalWhenHungry;

    [Space(10.0f)]
    public float DepressionMedsWearOffAfterHours = 26.0f;
    public float UntreatedDepressionHealthPenaltyPerHour;

    [Space(10.0f)]
    public float StatTransitionSpeedPerHour = 1.0f;

    [Space(10.0f)]
    public bool IsAtWork = false;

    [Space(10.0f)]
    public Trigger CurrentTrigger;
    public Begging CurrentBeggingSpot;
    public ClothingItem CurrentClothing;
    
    // Returns true if the player can currently afford the given price.
    public bool CanAfford(float price)
    {
        return (Money >= price);
    }
    
    // Completes all drink alcohol objectives.
    public void CompleteDrinkAlcoholObjective()
    {
        foreach (Objective objective in alcoholObjectives)
        {
            objective.Achieved = true;
        }
        alcoholObjectives.Clear();
    }

    // Changes the morale level by the given amount. 
    // If it's a positive change, morale gained also optionally reduces addiction.
    // If amount is negative, morale is lost.
    public void ChangeMorale(float amount, bool reduceAddiction = true)
    {
        moraleChangeRemaining += amount;

        // Only reduce addiction if we're gaining morale (morale isn't full).
        if (reduceAddiction && amount > 0.0f && Morale + moraleChangeRemaining < 1.0f)
        {
            Addiction -= amount * AddictionReducedPerMoraleGainedFactor;
        }
    }

    // Changes the nutrition level by the given amount. 
    public void ChangeNutrition(float amount)
    {
        nutritionChangeRemaining += amount;
    }

    // Changes the health level by the given amount. 
    public void ChangeHealthTiredness(float amount)
    {
        healthChangeRemaining += amount;
    }
    
    // Temporarily highlights the morale stat for the given time.
    // Use true for positive to indicate a positive effect on the stat.
    public void HighlightMoraleStatForTime(bool positive, float seconds)
    {
        if (positive)
        {
            highlightMoralePositive = true;
            highlightMoraleNegative = false;
        }
        else
        {
            highlightMoralePositive = false;
            highlightMoraleNegative = true;
        }
        highlightMoraleForSeconds = seconds;
        timeAtMoraleHighlight = Time.time;
    }

    // Temporarily highlights the nutrition stat for the given time.
    // Use true for positive to indicate a positive effect on the stat.
    public void HighlightNutritionStatForTime(bool positive, float seconds)
    {
        if (positive)
        {
            highlightNutritionPositive = true;
            highlightNutritionNegative = false;
        }
        else
        {
            highlightNutritionPositive = false;
            highlightNutritionNegative = true;
        }
        highlightNutritionForSeconds = seconds;
        timeAtNutritionHighlight = Time.time;
    }

    // Temporarily highlights the health stat for the given time.
    // Use true for positive to indicate a positive effect on the stat.
    public void HighlightHealthStatForTime(bool positive, float seconds)
    {
        if (positive)
        {
            highlightHealthPositive = true;
            highlightHealthNegative = false;
        }
        else
        {
            highlightHealthPositive = false;
            highlightHealthNegative = true;
        }
        highlightHealthForSeconds = seconds;
        timeAtHealthHighlight = Time.time;
    }

    // Treats depression for today.
    public void TreatDepressionToday()
    {
        IsDepressionTreated = true;
        hoursSinceDepressionLastTreated = 0.0f;
    }

    void Update () {
        float gameTimeDelta = 1.0f / 60.0f / 60.0f * Time.deltaTime * Main.GameTime.TimeScale;

        if (Main.SleepManager.IsAsleep)
        {
            // Nutrition and morale drop at a different rate while asleep.
            Nutrition -= NutritionLossPerHour * SleepNutritionGainFactor * gameTimeDelta;
            Morale -= MoraleLossPerHour * SleepMoraleLossFactor * gameTimeDelta;
        }
        else if (IsAtWork)
        {
            // Nutrition and health drop at a different rate while at work.
            Nutrition -= NutritionLossPerHour * WorkNutritionGainFactor * gameTimeDelta;
            HealthTiredness -= HealthLossPerHour * WorkHealthLossFactor * gameTimeDelta;
            Morale -= MoraleLossPerHour * WorkMoraleLossFactor * gameTimeDelta;
        }
        else
        {
            // Apply base changes.
            Nutrition -= NutritionLossPerHour * gameTimeDelta;
            HealthTiredness -= HealthLossPerHour * gameTimeDelta;
            Morale -= MoraleLossPerHour * gameTimeDelta;
        }

        // Nutrition goes down slower when satiated.
        if (Nutrition > NutritionOkAtLevel)
        {
            Nutrition +=
                (Nutrition - NutritionOkAtLevel) / (1.0f - NutritionOkAtLevel) *
                MaxNutritionRewardPerHour * gameTimeDelta;
        }

        // Nutrition affects health.
        if (Nutrition < NutritionOkAtLevel)
        {
            HealthTiredness -=
                (NutritionOkAtLevel - Nutrition) / NutritionOkAtLevel *
                MaxNutritionHealthPenaltyPerHour * gameTimeDelta;
        }
        else if (Nutrition > NutritionOkAtLevel)
        {
            HealthTiredness +=
                (Nutrition - NutritionOkAtLevel) / (1.0f - NutritionOkAtLevel) *
                MaxNutritionHealthRewardPerHour * gameTimeDelta;
        }

        // Health affects morale.
        if (HealthTiredness < HealthGoodAtLevel)
        {
            Morale -=
                (HealthGoodAtLevel - HealthTiredness) / HealthGoodAtLevel *
                MaxPoorHealthMoralePenaltyPerHour * gameTimeDelta;
        }
        else if (HealthTiredness > HealthGoodAtLevel)
        {
            Morale +=
                (HealthTiredness - HealthGoodAtLevel) / (1.0f - HealthGoodAtLevel) *
                MaxGoodHealthMoraleRewardPerHour * gameTimeDelta;
        }

        // Reduce inebriation over time.
        Inebriation -= InebriationDecreasesPerHour * gameTimeDelta;

        // Handle alcohol effects.
        float combinedWalkFactor = 1.0f;
        if (Inebriation >= AlcoholEffectsStartAtInebriation)
        {
            float intoxication = (Inebriation - AlcoholEffectsStartAtInebriation) / (1.0f - AlcoholEffectsStartAtInebriation);

            // Walking wonky.
            if (WalkWonkeyWhenIntoxicated)
            {
                Main.PlayerCharacter.SetWonkyWalkAngle(WonkyWalkAngleAtMaxInebriationDegrees);
            }

            // Vomit at regular intervals if intoxicated.
            if (VomitWhenIntoxicated)
            {
                if (!hasVomited || (intoxication != 0.0f &&
                     Time.time - timeAtLastVomit - Main.PlayerCharacter.VomitDurationSeconds >
                     VomitIntervalAtMaxInebriationSeconds / intoxication))
                {
                    hasVomited = true;
                    timeAtLastVomit = Time.time;

                    Main.PlayerCharacter.Vomit();
                }
            }

            // Walk slower.
            if (WalkSlowerWhenIntoxicated)
            {
                combinedWalkFactor *= WalkSpeedFactorAtMaxInebriation * intoxication + (1.0f - intoxication);
            }
        }
        else
        {
            Main.PlayerCharacter.SetWonkyWalkAngle(0.0f);
        }

        // Addiction affects morale.
        Morale -= Addiction * MaxAddictionMoralePenaltyPerHour * gameTimeDelta;

        // Spawn alcohol objectives sometimes while addicted.
        // Don't spawn while already heavily intoxicated.
        if (Addiction >= SpawnObjectivesAboveAddictionLevel && Inebriation < AlcoholEffectsStartAtInebriation)
        {
            float addictionLevel = (Addiction - SpawnObjectivesAboveAddictionLevel) / (1.0f - SpawnObjectivesAboveAddictionLevel);

            if (!hasSpawnedObjective || (addictionLevel != 0.0f &&
                Time.time - timeAtLastObjectiveSpawn > ObjectiveSpawnIntervalAtMaxAddictionSeconds / addictionLevel))
            {
                hasSpawnedObjective = true;
                timeAtLastObjectiveSpawn = Time.time;
                
                alcoholObjectives.Add(Main.ObjectiveList.NewObjective("Drink more alcohol"));
            }
        }
        
        // Handle poor health effects.
        if (HealthTiredness < PoorHealthEffectsBelowLevel)
        {
            float unwellness = (PoorHealthEffectsBelowLevel - HealthTiredness) / PoorHealthEffectsBelowLevel;
            
            // Walk slower.
            if (WalkSlowerWhenHealthIsPoor)
            {
                combinedWalkFactor *= WalkSpeedFactorAtWorstHealth * unwellness + (1.0f - unwellness);
            }

            // Cough at regular intervals.
            if (CoughWhenHealthIsPoor)
            {
                if (!hasCoughed || (unwellness != 0.0f &&
                     Time.time - timeAtLastCough - Main.PlayerCharacter.CoughDurationSeconds >
                     CoughIntervalAtWorstHealth / unwellness))
                {
                    hasCoughed = true;
                    timeAtLastCough = Time.time;
                    Main.PlayerCharacter.Cough();
                }
            }
        }

        // Poor morale.
        if (Morale < PoorMoraleEffectsBelowLevel)
        {
            float depression = (PoorMoraleEffectsBelowLevel - Morale) / PoorMoraleEffectsBelowLevel;

            // Walk slower.
            if (WalkSlowerWhenMoraleIsPoor)
            {
                combinedWalkFactor *= WalkSpeedFactorAtLowestMorale * depression + (1.0f - depression);
            }
        }

        // Apply combined walking factor.
        Main.PlayerCharacter.SetWalkSpeedFactor(combinedWalkFactor);

        // Player character stomach growls at a regular interval when hungry.
        if (Nutrition < StomachGrowlBelowNutrition)
        {
            if (!hasStomachGrowled || Time.time - timeAtLastStomachGrowl >= StomachGrowlIntervalWhenHungry)
            {
                hasStomachGrowled = true;
                timeAtLastStomachGrowl = Time.time;
                Main.PlayerCharacter.StomachGrowl();
            }
        }
        
        // Depression treatment wears off after some time.
        hoursSinceDepressionLastTreated += Main.GameTime.GameTimeDelta;
        if (hoursSinceDepressionLastTreated > DepressionMedsWearOffAfterHours)
        {
            IsDepressionTreated = false;
        }

        // Untreated depression affects morale.
        if (!IsDepressionTreated)
        {
            ChangeMorale(-UntreatedDepressionHealthPenaltyPerHour * gameTimeDelta);
        }

        // Highlight stats.
        var stats = Main.StatPanel;
        stats.NutritionText.fontStyle = FontStyle.Normal;
        stats.HealthText.fontStyle = FontStyle.Normal;
        stats.MoraleText.fontStyle = FontStyle.Normal;
        if (nutritionChangeRemaining > 0.0f || 
            (highlightNutritionPositive && Time.time - timeAtNutritionHighlight < highlightNutritionForSeconds))
        {
            stats.NutritionText.color = HighlightTextColour;
        }
        else if (nutritionChangeRemaining < 0.0f ||
            (highlightNutritionNegative && Time.time - timeAtNutritionHighlight < highlightNutritionForSeconds) ||
            Nutrition <= NutritionWarningThreshold)
        {
            stats.NutritionText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                stats.NutritionText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            stats.NutritionText.color = NormalTextColour;
        }
        if (healthChangeRemaining > 0.0f ||
            (highlightHealthPositive && Time.time - timeAtHealthHighlight < highlightHealthForSeconds))
        {
            stats.HealthText.color = HighlightTextColour;
        }
        else if (healthChangeRemaining < 0.0f || 
            (highlightHealthNegative && Time.time - timeAtHealthHighlight < highlightHealthForSeconds) ||
            HealthTiredness <= HealthWarningThreshold)
        {
            stats.HealthText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                stats.HealthText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            stats.HealthText.color = NormalTextColour;
        }
        if (moraleChangeRemaining > 0.0f ||
            (highlightMoralePositive && Time.time - timeAtMoraleHighlight < highlightMoraleForSeconds))
        {
            stats.MoraleText.color = HighlightTextColour;
        }
        else if (moraleChangeRemaining < 0.0f || 
            (highlightMoraleNegative && Time.time - timeAtMoraleHighlight < highlightMoraleForSeconds) ||
            Morale <= MoraleWarningThreshold)
        {
            stats.MoraleText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                stats.MoraleText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            stats.MoraleText.color = NormalTextColour;
        }

        // Stats change slower during a time-jump so that the jump isn't so obvious. 
        float statTransitionSpeed = StatTransitionSpeedPerHour;
        if (Main.GameTime.IsTimeSkipping)
        {
            statTransitionSpeed = statTransitionSpeed / Main.GameTime.TimeSkipSpeed * Main.GameTime.NormalTimeScale;
        }

        // Gradually apply morale changes.
        if (moraleChangeRemaining > 0.0f)
        {
            float change = Mathf.Min(
                moraleChangeRemaining, statTransitionSpeed * gameTimeDelta);
            Morale += change;
            moraleChangeRemaining -= change;
        }
        else if (moraleChangeRemaining < 0.0f)
        {
            float change = Mathf.Min(
                Mathf.Abs(moraleChangeRemaining), statTransitionSpeed * gameTimeDelta);
            Morale -= change;
            moraleChangeRemaining += change;
        }

        // Gradually apply nutrition changes.
        if (nutritionChangeRemaining > 0.0f)
        {
            float change = Mathf.Min(
                nutritionChangeRemaining, statTransitionSpeed * gameTimeDelta);
            Nutrition += change;
            nutritionChangeRemaining -= change;
        }
        else if (nutritionChangeRemaining < 0.0f)
        {
            float change = Mathf.Min(
                Mathf.Abs(nutritionChangeRemaining), statTransitionSpeed * gameTimeDelta);
            Nutrition -= change;
            nutritionChangeRemaining += change;
        }
        
        // Gradually apply health changes.
        if (healthChangeRemaining > 0.0f)
        {
            float change = Mathf.Min(
                healthChangeRemaining, statTransitionSpeed * gameTimeDelta);
            HealthTiredness += change;
            healthChangeRemaining -= change;
        }
        else if (healthChangeRemaining < 0.0f)
        {
            float change = Mathf.Min(
                Mathf.Abs(healthChangeRemaining), statTransitionSpeed * gameTimeDelta);
            HealthTiredness -= change;
            healthChangeRemaining += change;
        }

        // Limit stats to range 0-1.
        if (Money < 0)
        {
            Money = 0;
        }
        Nutrition = Mathf.Clamp(Nutrition, 0.0f, 1.0f);
        HealthTiredness = Mathf.Clamp(HealthTiredness, 0.0f, 1.0f);
        Morale = Mathf.Clamp(Morale, 0.0f, 1.0f);
        Inebriation = Mathf.Clamp(Inebriation, 0.0f, 1.0f);
        Addiction = Mathf.Clamp(Addiction, 0.0f, 1.0f);
        CurrentClothingCleanliness = Mathf.Clamp(CurrentClothingCleanliness, 0.0f, 1.0f);
        
        // Update stat texts.
        if (Main.MoneyPanel.MoneyText)
        {
            Main.MoneyPanel.MoneyText.text = "$" + Money.ToString("F2");
        }
        if (stats.NutritionText)
        {
            stats.NutritionText.text = "Nutrition: " + (Nutrition * 100).ToString("f0") + "%";
        }
        if (stats.HealthText)
        {
            stats.HealthText.text = "Health/Tiredness: " + (HealthTiredness * 100).ToString("f0") + "%";
        }
        if (stats.MoraleText)
        {
            stats.MoraleText.text = "Morale: " + (Morale * 100).ToString("f0") + "%";
        }
    }
}
