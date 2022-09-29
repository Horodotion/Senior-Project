using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    health,
    damage,
    speed
}

// This allows us to have a single variable with both a StatType and a float for a value
[System.Serializable] public class StatValue
{
    public StatType stat;
    public float value;
}

[System.Serializable]
[CreateAssetMenu(menuName = "Stat set")]
public class Stats : ScriptableObject
{
    // These Arrays are unused, and are purely to allowed us to utilize them in the hierarchy
    public List<StatValue> baseStatList; // The starting stats of the player
    public List<StatValue> maxStatList; // The maximum a stat can be

    // The dictionaries to actually be used in game
    // Stat is what will be referenced to the player, and allows buffs and debuffs to occur numerically.
    public Dictionary<StatType, float> stat = new Dictionary<StatType, float> {};

    // BaseStat is used to try resetting the stats back to a set number
    // IE, if a speedbuff only lasts 5 seconds and then goes back to the normal speed.
    public Dictionary<StatType, float> baseStat = new Dictionary<StatType, float> {};

    //MaxStat is to put a hard cap on a certain stat. IE, getting 150 points of healing but we can only have 100 health.
    public Dictionary<StatType, float> maxStat = new Dictionary<StatType, float> {};

    // This function that tells each dictionary to piece together
    public void SetStats()
    {
        stat = NewStatDictionary(baseStatList);
        baseStat = NewStatDictionary(baseStatList);
        maxStat = NewStatDictionary(maxStatList);
    }

    public void ResetStat(StatType statType)
    {
        stat[statType] = baseStat[statType];
    }

    // This function changes the stat to be called, up to the maximum set by the Maximum Stat
    public void AddToStat(StatType statType, float amountToAdd)
    {
        stat[statType] = Mathf.Clamp(stat[statType] + amountToAdd, 0f, maxStat[statType]);
    }

    // This is the function that pieces a dictionary from a list
    public Dictionary<StatType, float> NewStatDictionary(List<StatValue> statValues)
    {
        // This creates a new temporary dictionary to hold values
        Dictionary<StatType, float> dict = new Dictionary<StatType, float> {};

        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            dict.Add(statType, 0.0f);
        }

        // Looping through each value in the List, it takes the stattype as the Key, and the value as the value to add new values to the dictionary.
        foreach (StatValue stat in statValues)  // (int i = 0; i < statValues.Count; i++)
        {
            dict[stat.stat] = stat.value;
        }

        return dict;
    }

}