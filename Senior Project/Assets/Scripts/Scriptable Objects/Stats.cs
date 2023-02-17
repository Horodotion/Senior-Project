using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    newStat,
    health,
    damage,
    speed,
    temperature
}

[System.Serializable] public class CustomStats : SerializableDictionary<StatType, float> { }

[System.Serializable] public class StatValue
{
    public StatType statType;
    public float statValue;
}

[System.Serializable] public class IndividualStat
{
    [Tooltip("Stat is what is used by the player")]
    public float stat;
    [Tooltip("Stat is what is used to reset the player's original Stat")]
    public float baseStat;
    [Tooltip("The lowest value a stat can be.")]
    public float minimum = -Mathf.Infinity;
    [Tooltip("The highest value a stat can be.")]
    public float maximum = Mathf.Infinity;


    // This function simply sets stat to its baseStat counterpart
    public void ResetStat()
    {
        stat = baseStat;
    }

    // This function changes the stat to be called, up to the maximum set by the Maximum Stat
    public void AddToStat(float amountToAdd)
    {
        stat = Mathf.Clamp(stat + amountToAdd, minimum, maximum);
    }
}

[System.Serializable]
[CreateAssetMenu(menuName = "Stat set")]
public class Stats : ScriptableObject
{
    public List<StatValue> baseStatValues;
    public List<StatValue> maxStatValues;
    public List<StatValue> minStatValues;

    // The dictionaries to actually be used in game
    // Stat is what will be referenced to the player, and allows buffs and debuffs to occur numerically.
    [Tooltip("Stat is what is used by the player, it does not need to be edited directly.")]
    public CustomStats stat = new CustomStats {};

    // BaseStat is used to try resetting the stats back to a set number
    // IE, if a speedbuff only lasts 5 seconds and then goes back to the normal speed.
    [Tooltip("The base stats of the player, it will determine what Stat is set to by default and what it gets reset to.")]
    public CustomStats baseStat = new CustomStats {};

    [Tooltip("The minimum a stat can be, by default it is 0.")]
    // MinStat is used in case there is a minimum that is not 0, IE, scales from -100 to 100 instead of 0 to 100.
    public CustomStats minStat = new CustomStats {};

    [Tooltip("The maximum a stat can be, if 0, it will be ignored.")]
    //MaxStat is to put a hard cap on a certain stat. IE, getting 150 points of healing but we can only have 100 health.
    public CustomStats maxStat = new CustomStats {};

    // This function that tells each dictionary to piece together
    public void SetStats()
    {
        stat = NewStatDictionary(baseStatValues);
        baseStat = NewStatDictionary(baseStatValues);
        maxStat = NewStatDictionary(maxStatValues);
        minStat = NewStatDictionary(minStatValues);

        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            stat[statType] = baseStat[statType];
        }
    }

    // This function simply sets stat to its baseStat counterpart
    public void ResetStat(StatType statType)
    {
        stat[statType] = baseStat[statType];
    }

    // This function changes the stat to be called, up to the maximum set by the Maximum Stat
    public void AddToStat(StatType statType, float amountToAdd)
    {
        stat[statType] = Mathf.Clamp(stat[statType] + amountToAdd, minStat[statType], GetMaxStat(statType, maxStat));
    }


    // Gets the maximum a stat can have, in the case that it is needed
    public float GetMaxStat(StatType statType, CustomStats statsToGet)
    {
        if (statsToGet[statType] == 0)
        {
            return Mathf.Infinity;
        }
        else
        {
            return statsToGet[statType];
        }
    }

    public CustomStats NewStatDictionary(List<StatValue> statValues)
    {
        CustomStats newDict = new CustomStats();

        foreach(StatValue statValue in statValues)
        {
            newDict.Add(statValue.statType, statValue.statValue);
        }
        FillInBlankStats(newDict);

        return newDict;
    }

    // This function simply fills out the remaining values a dictionary has
    // It's mostly to avoid errors in cases where a stat is forgotten or ignored
    public void FillInBlankStats(CustomStats dict)
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            if (!dict.ContainsKey(statType))
            {
                dict.Add(statType, 0.0f);
            }
        }
    }
}