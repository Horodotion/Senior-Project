using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Ice Melee Attack")]
public class IceMeleeAttacks : AttackMotion
{
    //[SerializeField] GameObject hitBox;
    [SerializeField] float chargeSpeed;
    [SerializeField] float meleeDistance = 1.5f;
    [SerializeField] float turnSpeed = 1.5f;
    [SerializeField] float stoppingDistance = 1;
    [SerializeField] float hitboxSpawnTime = 0.5f;
    [SerializeField] float waitTimeAfterMelee = 0.5f;


    public override IEnumerator AttackingPlayer()
    {
        //yield return new WaitForSeconds(2f);
        //Debug.Log("5 " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;
        //Debug.Log("6 " + enemy.bossState);
        //yield return new WaitForSeconds(5f);
        
        enemy.navMeshAgent.stoppingDistance = stoppingDistance;
        while (!enemy.IsPlayerWithinDistance(meleeDistance))
        {
            enemy.navMeshAgent.speed = chargeSpeed;
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);

            yield return null;
        }
        //enemy.bossState = BossState.inCombat;

        
        yield return null;

        enemy.animator.SetBool("isMeleeAttacking", true);
        enemy.animator.SetFloat("element", 0);

        //enemy.navMeshAgent.speed = 0f;

        enemy.navMeshAgent.isStopped = true;


        while (enemy.animator.GetBool("isMeleeAttacking"))
        {
            
            for (float timer = 0; true; timer += Time.deltaTime)
            {
                enemy.AimTowards(PlayerController.puppet.transform.position, turnSpeed);
                if (timer > 0.2)
                {
                    break;
                }
                yield return null;
            }

            if (SP[0].gameObject.TryGetComponent<HitBoxController>(out HitBoxController hitbox))
            {
                hitbox.damage = -damage;
            }

            yield return null;
        }
        //yield return new WaitForSeconds(1f);
        /*
        for (float timer = 0; true; timer += Time.deltaTime)
        {
            enemy.AimTowards(PlayerController.puppet.transform.position, turnSpeed);
            if (timer > 0.2)
            {
                break;
            }
            yield return null;
        }
        
        if (SP[0].gameObject.TryGetComponent<HitBoxController>(out HitBoxController hitbox))
        {
            hitbox.damage = -damage;
        }
        
         */
        //SP[0].gameObject.GetComponent<HitBoxController>().damage = -damage;
        //yield return new WaitForSeconds(hitboxSpawnTime);
        //SP[0].gameObject.SetActive(false);

        //yield return new WaitForSeconds(waitTimeAfterMelee);

        enemy.navMeshAgent.isStopped = false;

        enemy.navMeshAgent.speed = enemy.speed;
        enemy.navMeshAgent.stoppingDistance = 0;

        ExitMeleeAttack();
        //enemy.bossState = BossState.inCombat;
    }

    public void SetHitBoxActive()
    {
        SP[0].gameObject.SetActive(true);
    }

    public void SetHitBoxInactive()
    {
        SP[0].gameObject.SetActive(false);
    }

    public void ExitAttackAnimation()
    {
        enemy.animator.SetBool("isMeleeAttacking", false);
    }
}
