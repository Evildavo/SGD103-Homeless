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

    public Main Main;

    [Header("Display settings:")]
    public Color NormalTextColour = Color.black;
    public Color WarningTextColour = Color.red;
    public Color HighlightTextColour = Color.yellow;
    public bool BoldTextDuringWarning = true;
    public float HungerWarningThreshold = 0.2f;
    public float HealthWarningThreshold = 0.2f;
    public float MoraleWarningThreshold = 0.2f;

    [Header("Stats:")]
    public float Money = 0;
    [Range(0.0f, 1.0f)]
    public float HungerThirstSatiety = 1.0f;
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
    public float HungerGainPerHour = 0.0f;        
    public float HealthLossPerHour = 0.0f;
    public float MoraleLossPerHour = 0.0f;
    
    [Space(10.0f)]
    public float MaxHungerHealthPenaltyPerHour = 0.0f;
    [Range(0.0f, 1.0f)]
    public float HungerSatiatedAtLevel = 0.0f;
    public float MaxSatietyHealthRewardPerHour = 0.0f;

    [Header("Hunger goes down slower when satiated")]
    public float MaxSatietyHungerRewardPerHour = 0.0f;

    [Space(10.0f)]
    public float MaxPoorHealthMoralePenaltyPerHour = 0.0f;
    [Range(0.0f, 1.0f)]
    public float HealthGoodAtLevel = 0.0f;
    public float MaxGoodHealthMoraleRewardPerHour = 0.0f;

    [Space(10.0f)]
    public float SleepHungerGainFactor = 0.0f;
    public float SleepMoraleLossFactor = 0.0f;
    [Header("Not counting bonus from a sleeping bag")]
    public float MinSleepingRoughHealthGainPerHour = 0.0f;
    public float MaxSleepingRoughHealthGainPerHour = 0.0f;

    [Space(10.0f)]
    public float WorkHungerGainFactor = 1.0f;
    public float WorkHealthLossFactor = 1.0f;
    public float WorkMoraleLossFactor = 1.0f;    
    [Header("See Job Locations for morale gain settings.")]

    [Space(10.0f)]
    public float InebriationDecreasesPerHour;
    public float AlcoholEffectsStartAtInebriation = 1.0f;
    [Header("Turns slightly while walking")]
    public bool WalkWonkeyWhenIntoxicated;
    public bool VomitWhenIntoxicated;
    public bool WalkSlowerWhenIntoxicated;
    public float WonkyWalkAngleAtMaxInebriationDegrees;
    public float VomitIntervalAtMaxInebriationSeconds;
    public float HungerSatietyLostPerVomit;
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
    public float StomachGrowlBelowHungerSatiety;
    public float StomachGrowlIntervalWhenHungry;

    [Space(10.0f)]
    public float DepressionMedsWearOffAfterHours = 26.0f;
    public float UntreatedDepressionHealthPenaltyPerHour;

    [Space(10.0f)]
    public bool IsAtWork = false;

    [Space(10.0f)]
    public Trigger CurrentTrigger;

    [Space(10.0f)]
    public bool HighlightHungerThirst = false;
    public bool HighlightHealth = false;
    public bool HighlightMorale = false;

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

    // Changes the morale by the given amount. 
    // If it's a positive change, morale gained also optionally reduces addiction.
    // If amount is negative, morale is lost.
    public void ChangeMorale(float amount, bool reduceAddiction = true)
    {
        Morale += amount;

        // Only reduce addiction if we're gaining morale (morale isn't full).
        if (reduceAddiction && amount > 0.0f && Morale < 1.0f)
        {
            Addiction -= amount * AddictionReducedPerMoraleGainedFactor;
        }
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
            // Hunger and morale drop at a different rate while asleep.
            HungerThirstSatiety -= HungerGainPerHour * SleepHungerGainFactor * gameTimeDelta;
            Morale -= MoraleLossPerHour * SleepMoraleLossFactor * gameTimeDelta;

            // Gain health based on sleep quality.
            float minHealthGain = MinSleepingRoughHealthGainPerHour;
            float maxHealthGain = MaxSleepingRoughHealthGainPerHour;
            HealthTiredness += (minHealthGain + Main.SleepManager.SleepQuality * (maxHealthGain - minHealthGain)) * gameTimeDelta;
        }
        else if (IsAtWork)
        {
            // Hunger and health drop at a different rate while at work.
            HungerThirstSatiety -= HungerGainPerHour * WorkHungerGainFactor * gameTimeDelta;
            HealthTiredness -= HealthLossPerHour * WorkHealthLossFactor * gameTimeDelta;
            Morale -= MoraleLossPerHour * WorkMoraleLossFactor * gameTimeDelta;
        }
        else
        {
            // Apply base changes.
            HungerThirstSatiety -= HungerGainPerHour * gameTimeDelta;
            HealthTiredness -= HealthLossPerHour * gameTimeDelta;
            Morale -= MoraleLossPerHour * gameTimeDelta;
        }

        // Hunger goes down slower when satiated.
        if (HungerThirstSatiety > HungerSatiatedAtLevel)
        {
            HungerThirstSatiety +=
                (HungerThirstSatiety - HungerSatiatedAtLevel) / (1.0f - HungerSatiatedAtLevel) *
                MaxSatietyHungerRewardPerHour * gameTimeDelta;
        }

        // Hunger affects health.
        if (HungerThirstSatiety < HungerSatiatedAtLevel)
        {
            HealthTiredness -=
                (HungerSatiatedAtLevel - HungerThirstSatiety) / HungerSatiatedAtLevel *
                MaxHungerHealthPenaltyPerHour * gameTimeDelta;
        }
        else if (HungerThirstSatiety > HungerSatiatedAtLevel)
        {
            HealthTiredness +=
                (HungerThirstSatiety - HungerSatiatedAtLevel) / (1.0f - HungerSatiatedAtLevel) *
                MaxSatietyHealthRewardPerHour * gameTimeDelta;
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

                    HungerThirstSatiety -= HungerSatietyLostPerVomit;
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
        if (HungerThirstSatiety < StomachGrowlBelowHungerSatiety)
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
            Morale -= UntreatedDepressionHealthPenaltyPerHour * gameTimeDelta;
        }

        // Limit stats to range 0-1.
        if (Money < 0)
        {
            Money = 0;
        }
        HungerThirstSatiety = Mathf.Clamp(HungerThirstSatiety, 0.0f, 1.0f);
        HealthTiredness = Mathf.Clamp(HealthTiredness, 0.0f, 1.0f);
        Morale = Mathf.Clamp(Morale, 0.0f, 1.0f);
        Inebriation = Mathf.Clamp(Inebriation, 0.0f, 1.0f);
        Addiction = Mathf.Clamp(Addiction, 0.0f, 1.0f);
        CurrentClothingCleanliness = Mathf.Clamp(CurrentClothingCleanliness, 0.0f, 1.0f);

        // Highlight stats.
        var stats = Main.StatPanel;
        stats.HungerThirstText.fontStyle = FontStyle.Normal;
        stats.HealthText.fontStyle = FontStyle.Normal;
        stats.MoraleText.fontStyle = FontStyle.Normal;
        if (HighlightHungerThirst)
        {
            stats.HungerThirstText.color = HighlightTextColour;
        }
        else if (HungerThirstSatiety <= HungerWarningThreshold)
        {
            stats.HungerThirstText.color = WarningTextColour;
            if (BoldTextDuringWarning)
            {
                stats.HungerThirstText.fontStyle = FontStyle.Bold;
            }
        }
        else
        {
            stats.HungerThirstText.color = NormalTextColour;
        }
        if (HighlightHealth)
        {
            stats.HealthText.color = HighlightTextColour;
        }
        else if (HealthTiredness <= HealthWarningThreshold)
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
        if (HighlightMorale)
        {
            stats.MoraleText.color = HighlightTextColour;
        }
        else if (Morale <= MoraleWarningThreshold)
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

        // Update stat texts.
        if (Main.MoneyPanel.MoneyText)
        {
            Main.MoneyPanel.MoneyText.text = "$" + Money.ToString("F2");
        }
        if (stats.HungerThirstText)
        {
            stats.HungerThirstText.text = "Hunger/Thirst: " + (HungerThirstSatiety * 100).ToString("f0") + "%";
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
