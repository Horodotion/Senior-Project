using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
// This allows us to have a single variable with both a StatType and a float for a value
[System.Serializable] public class EnemyAttackValue
{
    public int damage;
    public float coolDown;
    public float timeTaken;
    public float MinDistanceTowardPlayer;
    public float MaxDistanceTowardPlayer;
}
*/

[System.Serializable]
[CreateAssetMenu(menuName = "Enemy Attack Value")]
public class AttackMotion : ScriptableObject
{
    [HideInInspector] public PlayerController player;
    [HideInInspector] public BossEnemyController enemy;

    public int damage;
    public float coolDown;
    public float timeTaken;
    public float minDistanceTowardPlayer;
    public float maxDistanceTowardPlayer;
    protected Transform [] SP;
    protected bool isFiredOnce; // Check if the attack has been fired

    //[SerializeField] GameObject enemyGameObject;

    //EnemyAttackValue attackValue;

    //public AttacksManager attacksManager;


    public virtual void Awake()
    {
        player = PlayerController.instance;
        isFiredOnce = true;
        //attacksManager = AttacksManager.instance;
        //enemy = enemyGameObject.GetComponent<MovingEnemyController>();
    }

    public virtual void InitializeAttacks(BossEnemyController enemyController, Transform[] SP)
    {
        enemy = enemyController;
        this.SP = SP;
    }

    public virtual IEnumerator AttackingPlayer()
    {
        yield return null;
    }

    public bool AbleToAttack(float timer)
    {

        return IsPlayerWithinDistance(maxDistanceTowardPlayer) && 
              !IsPlayerWithinDistance(minDistanceTowardPlayer) &&
              timer <= 0;

    }
    public bool IsPlayerWithinDistance(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, range);
        foreach (Collider thisCollider in colliders)
        {
            if (thisCollider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }
}


