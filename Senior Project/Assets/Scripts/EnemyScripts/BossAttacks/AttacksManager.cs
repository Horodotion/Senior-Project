using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct EnemyAttack
{
    public AttackMotion attackMotion;
    public GameObject[] spawnPoiont;
}

public class AttacksManager : MonoBehaviour
{
    [HideInInspector] public BossEnemyController enemy;

    public AttacksManager instance;
    
    public float timer;

    //[SerializeField] public AttackWithSP[] attacksList;
    //[SerializeField] public EnemyAttacks currentAttack;

    //All the boss attack
    [SerializeField] public EnemyAttack iceMeleeAttack;
    [SerializeField] public EnemyAttack fireMeleeAttack;
    [SerializeField] public EnemyAttack iceRangedAttack;
    [SerializeField] public EnemyAttack fireRangedAttack;
    [SerializeField] public EnemyAttack laserAttack;

    //All the attack decision and modifer
    [SerializeField] public AttackDecision rangedAttackDicision;
    [SerializeField] public AttackDecision[] rangedAttackDicisionMod = new AttackDecision[4];

    [SerializeField] public AttackDecision meleeAttackDicision;
    [SerializeField] public AttackDecision[] meleeAttackDicisionMod = new AttackDecision[4];


    private bool ableToAttack = true;

    public void Awake()
    {
        enemy = GetComponent<BossEnemyController>();

        // Initialize the attacks so that the attackmotion class contain their spawn point
        iceMeleeAttack.attackMotion.InitializeAttacks(enemy, iceMeleeAttack.spawnPoiont);
        fireMeleeAttack.attackMotion.InitializeAttacks(enemy, fireMeleeAttack.spawnPoiont);
        iceRangedAttack.attackMotion.InitializeAttacks(enemy, iceRangedAttack.spawnPoiont);
        fireRangedAttack.attackMotion.InitializeAttacks(enemy, fireRangedAttack.spawnPoiont);
        laserAttack.attackMotion.InitializeAttacks(enemy, laserAttack.spawnPoiont);
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

    //Decide which element for the ranged attack
    public IEnumerator RangedAttack()
    {
        // Decide if it fire or ice
        if (RangedAttackDicision())
        {
            //Use Ice
            Debug.Log("Use ice projectile");
            return iceRangedAttack.attackMotion.AttackingPlayer();
        }
        else
        {
            //Use Fire
            Debug.Log("Use fire projectile");
            return fireRangedAttack.attackMotion.AttackingPlayer();
        }
        //StartCoroutine(enemy.MovementCoroutine);
    }
    public IEnumerator LaserAttack()
    {
        return laserAttack.attackMotion.AttackingPlayer();

    }
    //Decide which element for the melee attack
    public IEnumerator MeleeAttack()
    {
        // Decide if it fire or ice
        if (MeleeAttackDicision())
        {
            Debug.Log("Use ice melee attack");
            //Use Ice
            return iceMeleeAttack.attackMotion.AttackingPlayer();
        }
        else
        {
            //Use Fire
            Debug.Log("Use fire melee attack");
            return fireMeleeAttack.attackMotion.AttackingPlayer();
        }
        //StartCoroutine(enemy.MovementCoroutine);
    }

    //This output a bool (true is ice/ false is fire) by calculate the element needed to use using the decision and decision modifier during range attack.
    public bool RangedAttackDicision()
    {
        AttackDecision temp = new AttackDecision(rangedAttackDicision);

        // Adds up all the modifier and calculate the weight of each elements for the ranged attack.
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
        //Find which element for the next attack
        return temp.GiveTheNextRandomDicision(); 
    }

    //This output a bool (true is ice/ false is fire) by calculate the element needed to use using the decision and decision modifier during melee attack.
    public bool MeleeAttackDicision()
    {
        AttackDecision temp = new AttackDecision(meleeAttackDicision);

        // Adds up all the modifier and calculate the weight of each elements for the melee attack.
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

        //Find which element for the next attack
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
