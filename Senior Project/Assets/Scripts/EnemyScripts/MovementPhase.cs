using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Phase/MovementPhase")]
[System.Serializable]
public class MovementPhase : ScriptableObject
{
    public float hPPercentageToEnterPhase;

    [Header("Boss Phase Starting")]
    //[SerializeField] public MovementDecision startingDecision;
    public bool isArmored;

    [ToggleableVarable("isArmored")] public DamageType startingArmorType;
    [ToggleableVarable("isArmored")] public IndividualStat armorStats;
    [ToggleableVarable("isArmored")] public float hPReductionPercentOnArmorBreak;

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
    [SerializeField] public MovementDecision tauntAttackDecision;
    [SerializeField] public MovementDecision getArmor;
    [HideInInspector] public MovementDecision privousDecision;
    private void OnEnable()
    {
        //getArmor = new MovementDecision(BossState.setArmor);
    }
    

}

