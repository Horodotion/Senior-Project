using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Decision/Attack Dicision")]
public class AttackDicision : Dicision
{
    public int iceAttack;
    public int fireAttack;
    public AttackDicision()
    {
    }
    public AttackDicision(int iA, int fA)
    {
        iceAttack = iA;
        fireAttack = fA;
    }
    public AttackDicision AddDicision(AttackDicision d)
    {
        return new AttackDicision(
                this.iceAttack + d.fireAttack,
                this.iceAttack + d.fireAttack);
    }
    public int AddAllDicision()
    {
        return iceAttack + fireAttack;
    }
    public bool GiveTheNextRandomDicision() //Return true if the output is ice, and false if it's fire.
    {
        int index = Random.Range(1, AddAllDicision());
        switch (index)
        {
            case int x when x > 0 && x <= iceAttack:
                return true;
            case int x when x > iceAttack && x <= AddAllDicision():
                return false;
        }
        Debug.Log("Out of bounds in GiveTheNextRandomDicision() for attack type");
        return false;
    }
}

public class Dicision : ScriptableObject
{
}

[System.Serializable]
public struct AttackWithSP
{
    public EnemyAttacks attacks;
    public Transform[] spawnPoiont;
}

public class AttacksManager : MonoBehaviour
{
    [HideInInspector] public BossEnemyController enemy;

    public AttacksManager instance;
    
    public float timer;

    [SerializeField] public AttackWithSP[] attacksList;
    [SerializeField] public EnemyAttacks currentAttack;

    [SerializeField] public EnemyAttacks iceMeleeAttack;
    [SerializeField] public EnemyAttacks fireMeleeAttack;
    [SerializeField] public ProjectileAttacks iceRangedAttack;
    [SerializeField] public ProjectileAttacks fireRangedAttack;


    private bool ableToAttack = true;

    public void Awake()
    {
        enemy = GetComponent<BossEnemyController>();
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
    }
    private void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
        else timer = 0;
    }
    public void Attack()
    {
        enemy.MovementCoroutine = StartCoroutine(RandomAttack().AttackingPlayer());
        
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
    public void Attack(EnemyAttacks attack)
    {
        if (attacksList != null)
        {
            if (currentAttack.AbleToAttack(timer))
            {
                //enemy.bossState = BossState.attacking;
                currentAttack.AttackingPlayer();
                timer = currentAttack.coolDown;
                currentAttack = attack;// Change to Next attack;
                timer += currentAttack.timeTaken;
            }
        }
    }
    /*
    public bool AbleToAttack(float timer)
    {
        //return 0 < currentAttack.coolDown + currentAttack.timeTaken;
    }
    */
    public EnemyAttacks RandomAttack()
    {
        if (attacksList.Length != 0)
        {
            return attacksList[Random.Range(0, attacksList.Length)].attacks;
        }
        return null;
    }

    

}
