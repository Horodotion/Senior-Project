using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct EnemyAttack
{
    public AttackMotion attackMotion;
    public Transform[] spawnPoiont;
}

public class AttacksManager : MonoBehaviour
{
    [HideInInspector] public BossEnemyController enemy;

    public AttacksManager instance;
    
    public float timer;

    //[SerializeField] public AttackWithSP[] attacksList;
    //[SerializeField] public EnemyAttacks currentAttack;

    [SerializeField] public EnemyAttack iceMeleeAttack;
    [SerializeField] public EnemyAttack fireMeleeAttack;
    [SerializeField] public EnemyAttack iceRangedAttack;
    [SerializeField] public EnemyAttack fireRangedAttack;

    AttackDecision rangedAttackDicision;
    [SerializeField] public AttackDecision[] rangedAttackDicisionMod = new AttackDecision[4];

    AttackDecision meleeAttackDicision;
    [SerializeField] public AttackDecision[] meleeAttackDicisionMod = new AttackDecision[4];


    private bool ableToAttack = true;

    public void Awake()
    {
        enemy = GetComponent<BossEnemyController>();

        iceMeleeAttack.attackMotion.InitializeAttacks(enemy, iceMeleeAttack.spawnPoiont);
        fireMeleeAttack.attackMotion.InitializeAttacks(enemy, fireMeleeAttack.spawnPoiont);
        iceRangedAttack.attackMotion.InitializeAttacks(enemy, iceRangedAttack.spawnPoiont);
        fireRangedAttack.attackMotion.InitializeAttacks(enemy, fireRangedAttack.spawnPoiont);
        /*
        currentAttack = RandomAttack();
        timer = currentAttack.coolDown;
        instance = this;
        if (attacksList != null)
        {
            foreach (AttackWithSP thisAttacks in attacksList)
            {
                thisAttacks.attacks.InitializeAttacks(enemy, thisAttacks.spawnPoiont);
            }
        }
        */
    }
    private void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
        else timer = 0;
    }
    public void Attack()
    {
        //enemy.MovementCoroutine = StartCoroutine(RandomAttack().AttackingPlayer());
        
        /*
        if (attacksList != null)
        {
            if (currentAttack.AbleToAttack(timer))
            {
                //enemy.bossState = BossState.attacking;
                currentAttack.AttackingPlayer();
                timer = currentAttack.coolDown;
                currentAttack = RandomAttack();// Change to Next attack;
            }
        }
        */
    }
    public void RangedAttack()
    {
        if (RangedAttackDicision())
        {
            enemy.MovementCoroutine = StartCoroutine(iceRangedAttack.attackMotion.AttackingPlayer());
        }
        else
        {
            enemy.MovementCoroutine = StartCoroutine(fireRangedAttack.attackMotion.AttackingPlayer());
        }
    }
    public void MeleeAttack()
    {
        if (MeleeAttackDicision())
        {
            enemy.MovementCoroutine = StartCoroutine(iceMeleeAttack.attackMotion.AttackingPlayer());
        }
        else
        {
            enemy.MovementCoroutine = StartCoroutine(fireMeleeAttack.attackMotion.AttackingPlayer());
        }
    }
    public bool RangedAttackDicision()
    {
        AttackDecision temp = rangedAttackDicision;
        if (PlayerController.instance.playerStats.stat[StatType.temperature] >= 75)
        {
            temp.AddDicision(rangedAttackDicisionMod[0]);
        }
        if (PlayerController.instance.playerStats.stat[StatType.temperature] >= 60)
        {
            temp.AddDicision(rangedAttackDicisionMod[1]);
        }
        if (PlayerController.instance.playerStats.stat[StatType.temperature] <= 40)
        {
            temp.AddDicision(rangedAttackDicisionMod[2]);
        }
        if (PlayerController.instance.playerStats.stat[StatType.temperature] <= 25)
        {
            temp.AddDicision(rangedAttackDicisionMod[3]);
        }
        return temp.GiveTheNextRandomDicision();
    }
    public bool MeleeAttackDicision()
    {
        AttackDecision temp = meleeAttackDicision;
        if (PlayerController.instance.playerStats.stat[StatType.temperature] >= 75)
        {
            temp.AddDicision(meleeAttackDicisionMod[0]);
        }
        if (PlayerController.instance.playerStats.stat[StatType.temperature] >= 60)
        {
            temp.AddDicision(meleeAttackDicisionMod[1]);
        }
        if (PlayerController.instance.playerStats.stat[StatType.temperature] <= 40)
        {
            temp.AddDicision(meleeAttackDicisionMod[2]);
        }
        if (PlayerController.instance.playerStats.stat[StatType.temperature] <= 25)
        {
            temp.AddDicision(meleeAttackDicisionMod[3]);
        }
        return temp.GiveTheNextRandomDicision();
    }
    
    /*
    public bool AbleToAttack(float timer)
    {
        //return 0 < currentAttack.coolDown + currentAttack.timeTaken;
    }
    
    public EnemyAttacks RandomAttack()
    {
        if (attacksList.Length != 0)
        {
            return attacksList[Random.Range(0, attacksList.Length)].attacks;
        }
        return null;
    }
    */


}
