using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Key")]
public class Key : ScriptableObject
{
    public KeyType key;
    public bool ConsumedOnUse = true;
    [HideInInspector] public int stackCount;
}
