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

    //[SerializeField] public EnemyAttack[] attacksList;
    //[SerializeField] public EnemyAttack currentAttack;

    //All the boss attack

    [SerializeField] public EnemyAttack iceMeleeAttack;
    [SerializeField] public EnemyAttack fireMeleeAttack;
    [SerializeField] public EnemyAttack iceRangedAttack;
    [SerializeField] public EnemyAttack fireRangedAttack;
    [SerializeField] public EnemyAttack iceLaserAttack;
    [SerializeField] public EnemyAttack fireLaserAttack;

    //All the attack decision and modifer
    [SerializeField] public AttackDecision rangedAttackDicision;
    [SerializeField] public AttackDecision[] rangedAttackDicisionMod = new AttackDecision[4];

    [SerializeField] public AttackDecision meleeAttackDicision;
    [SerializeField] public AttackDecision[] meleeAttackDicisionMod = new AttackDecision[4];

    public float leftRightHand = 0;
    private bool ableToAttack = true;

    public void Awake()
    {
        enemy = GetComponent<BossEnemyController>();
        //instance = GetComponent<AttacksManager>();
        leftRightHand = Random.Range(0, 2);

        // Initialize the attacks so that the attackmotion class contain their spawn point
        iceMeleeAttack.attackMotion.InitializeAttacks(enemy, iceMeleeAttack.spawnPoiont);
        fireMeleeAttack.attackMotion.InitializeAttacks(enemy, fireMeleeAttack.spawnPoiont);
        iceRangedAttack.attackMotion.InitializeAttacks(enemy, iceRangedAttack.spawnPoiont);
        fireRangedAttack.attackMotion.InitializeAttacks(enemy, fireRangedAttack.spawnPoiont);
        iceLaserAttack.attackMotion.InitializeAttacks(enemy, iceLaserAttack.spawnPoiont);
        fireLaserAttack.attackMotion.InitializeAttacks(enemy, fireLaserAttack.spawnPoiont);
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

    private float ChangeHands()
    {
        if (leftRightHand == 0)
        {
            leftRightHand = 1;
        }
        else
        {
            leftRightHand = 0;
        }
        return leftRightHand;
    }

    //Decide which element for the ranged attack
    public IEnumerator RangedAttack()
    {
        
        // Decide if it fire or ice
        if (RangedAttackDicision())
        {
            //Use Ice
            Debug.Log("Use ice projectile");
            return iceRangedAttack.attackMotion.AttackingPlayer(ChangeHands());
        }
        else
        {
            //Use Fire
            Debug.Log("Use fire projectile");
            return fireRangedAttack.attackMotion.AttackingPlayer(ChangeHands());
        }
        
        //StartCoroutine(enemy.MovementCoroutine);
    }

    public IEnumerator LaserAttack()
    {
        if (RangedAttackDicision())
        {
            //Use Ice
            Debug.Log("Use ice projectile");
            return iceLaserAttack.attackMotion.AttackingPlayer();
        }
        else
        {
            //Use Fire
            Debug.Log("Use fire projectile");
            return fireLaserAttack.attackMotion.AttackingPlayer();
        }

    }

    //Decide which element for the melee attack
    public IEnumerator MeleeAttack()
    {
        // Decide if it fire or ice
        if (MeleeAttackDicision())
        {
            Debug.Log("Use ice melee attack");
            //Use Ice
            return iceMeleeAttack.attackMotion.AttackingPlayer(ChangeHands());
        }
        else
        {
            //Use Fire
            Debug.Log("Use fire melee attack");
            return fireMeleeAttack.attackMotion.AttackingPlayer(ChangeHands());
        }
        //StartCoroutine(enemy.MovementCoroutine);
    }
    
    //This output a bool (true is ice/ false is fire) by calculate the element needed to use using the decision and decision modifier during range attack.
    public bool RangedAttackDicision()
    {
        AttackDecision temp = new AttackDecision(rangedAttackDicision);

        // Adds up all the modifier and calculate the weight of each elements for the ranged attack.
        if (PlayerController.instance.temperature.stat >= 75)
        {
            temp.AddDicision(rangedAttackDicisionMod[0]);
        }
        if (PlayerController.instance.temperature.stat >= 60)
        {
            temp.AddDicision(rangedAttackDicisionMod[1]);
        }
        if (PlayerController.instance.temperature.stat <= 40)
        {
            temp.AddDicision(rangedAttackDicisionMod[2]);
        }
        if (PlayerController.instance.temperature.stat <= 25)
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
        if (PlayerController.instance.temperature.stat >= 75)
        {
            temp.AddDicision(meleeAttackDicisionMod[0]);
        }
        if (PlayerController.instance.temperature.stat >= 60)
        {
            temp.AddDicision(meleeAttackDicisionMod[1]);
        }
        if (PlayerController.instance.temperature.stat <= 40)
        {
            temp.AddDicision(meleeAttackDicisionMod[2]);
        }
        if (PlayerController.instance.temperature.stat <= 25)
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

    public void SetHitBox()
    {
        if (enemy.animator.GetFloat("element") == 0)
        {
            iceMeleeAttack.spawnPoiont[0].gameObject.SetActive(iceMeleeAttack.spawnPoiont[0].gameObject.activeSelf ? false : true);
        }
        else
        {
            fireMeleeAttack.spawnPoiont[0].gameObject.SetActive(iceMeleeAttack.spawnPoiont[0].gameObject.activeSelf ? false : true);
        }
    }

    public void FireProjectile()
    {
        
        if (enemy.animator.GetFloat("element") == 0)
        {
            GameObject thisProjectile1 = SpawnManager.instance.GetGameObject(((IceProjectileAttacks)iceRangedAttack.attackMotion).getProjectile(), SpawnType.projectile);
            //Debug.Log(thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController testController));
            if (thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController projectileController))
            {
                projectileController.transform.position = iceRangedAttack.spawnPoiont[0].transform.position;
                projectileController.transform.rotation = iceRangedAttack.spawnPoiont[0].transform.rotation;
                //SpawnManager.instance.GetGameObject(projectile, SpawnType.projectile);
                projectileController.LaunchProjectile();
            }
        }
        else
        {
            GameObject thisProjectile1 = SpawnManager.instance.GetGameObject(((FireProjectileAttacks)fireRangedAttack.attackMotion).getProjectile(), SpawnType.projectile);
            //Debug.Log(thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController testController));
            if (thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController projectileController))
            {
                projectileController.transform.position = fireRangedAttack.spawnPoiont[0].transform.position;
                projectileController.transform.rotation = fireRangedAttack.spawnPoiont[0].transform.rotation;
                //SpawnManager.instance.GetGameObject(projectile, SpawnType.projectile);
                projectileController.LaunchProjectile();
            }
        }
        GameObject projectile;
        
    }

    public void SetLaser()
    {
        if (enemy.animator.GetFloat("element") == 0)
        {
            iceMeleeAttack.spawnPoiont[0].gameObject.SetActive(iceMeleeAttack.spawnPoiont[0].gameObject.activeSelf ? false : true);
        }
        else
        {
            fireMeleeAttack.spawnPoiont[0].gameObject.SetActive(iceMeleeAttack.spawnPoiont[0].gameObject.activeSelf ? false : true);
        }
    }


    /*
    public void ExitAttackAnimation(string action)
    {
        if (action == "melee")
        {
            enemy.animator.SetBool("isMeleeAttacking", false);
        }
        if(action == "ranged")
        {
            Debug.Log(action);
            enemy.animator.SetBool("isRangedAttacking", false);
        }
        if (action == "laser")
        {
            enemy.animator.SetBool("isLaserAttacking", false);
        }
    }
    */


    public void ChangeLaserAniState()
    {
        if (enemy.animator.GetInteger(enemy.aniLaserState) == 3)
        {
            enemy.animator.SetInteger(enemy.aniLaserState, 0);
            ExitAttackAnimation();
        }
        else
        {
            enemy.animator.SetInteger(enemy.aniLaserState, enemy.animator.GetInteger(enemy.aniLaserState) + 1);
        }
    }
    public void ExitAttackAnimation()
    {
        enemy.IdleAni();
    }
}
