using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Phase/MovementPhase")]
[System.Serializable]
public class MovementPhase : ScriptableObject
{
    //[Header("Boss Dicision startingPoint")]
    //[SerializeField] public MovementDecision startingDecision;
    //public bool isArmored;

    //[ToggleableVarable("isArmored")] public DamageType startingArmorType;
    //[ToggleableVarable("isArmored")] public float armorHP;

    public float hPPercentageToEnterPhase;

    [Header("Boss Dicision")]
    [SerializeField] public MovementDecision meleeAttackDecision;
    [SerializeField] public MovementDecision orbwalkDecision;
    [SerializeField] public MovementDecision rangedAttackDecision;
    [SerializeField] public MovementDecision coverDecision;
    //[SerializeField] public MovementDecision coverDecisionMod;
    [SerializeField] public MovementDecision teleportDecision;
    //[SerializeField] public MovementDecision teleportDecisionMod;
    [SerializeField] public MovementDecision dropMineDecision;
    [SerializeField] public MovementDecision dropTurretDecision;
    [SerializeField] public MovementDecision spawnMobDecision;
    [SerializeField] public MovementDecision laserAttackDecision;
    [HideInInspector] public MovementDecision armorDecision;
    /*
    public void GetTheNextRandomDicision()
    {

    }
    */
}

