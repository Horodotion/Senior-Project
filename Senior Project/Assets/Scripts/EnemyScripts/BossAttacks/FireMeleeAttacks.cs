using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Fire Melee Attack")]
public class FireMeleeAttacks : AttackMotion
{
    //[SerializeField] GameObject hitBox;
    [SerializeField] float chargeSpeed;
    [SerializeField] float meleeDistance = 1.5f;
    [SerializeField] float turnSpeed = 1.5f;
    [SerializeField] float stoppingDistance = 1f;
    [SerializeField] float hitboxSpawnTime = 0.5f;
    [SerializeField] float waitTimeAfterMelee = 0.5f;


    public override IEnumerator AttackingPlayer(float leftRightHand)
    {
        //yield return new WaitForSeconds(2f);
        //Debug.Log("5 " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;
        // Debug.Log("6 " + enemy.bossState);
        //yield return new WaitForSeconds(5f);

        //enemy.navMeshAgent.speed = enemy.speed;
        //enemy.animator.SetInteger(enemy.aniDecision, enemy.runningAni);
        enemy.IdleAni();
        yield return null;

        while (!enemy.IsPlayerWithinDistance(meleeDistance))
        {
            enemy.RunningAni();
            enemy.navMeshAgent.stoppingDistance = stoppingDistance;
            enemy.navMeshAgent.speed = chargeSpeed;
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);

            yield return null;
        }
        //enemy.bossState = BossState.inCombat;

        enemy.IdleAni();
        yield return null;


        //enemy.animator.SetBool("isMeleeAttacking", true);
        enemy.animator.SetFloat(enemy.aniLeftRightDecision, leftRightHand);
        enemy.animator.SetInteger(enemy.aniDecision, enemy.meleeAni);
        enemy.animator.SetFloat("element", 1);

        //enemy.navMeshAgent.speed = 0f;
        enemy.navMeshAgent.isStopped = true;

        while (enemy.animator.GetInteger(enemy.aniDecision) == enemy.meleeAni)
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
                hitbox.damage = damage;
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

        SP[0].gameObject.SetActive(true);
        if (SP[0].gameObject.TryGetComponent<HitBoxController>(out HitBoxController hitbox))
        {
            hitbox.damage = damage;
        }
        yield return new WaitForSeconds(hitboxSpawnTime);
        SP[0].gameObject.SetActive(false);
        
        yield return new WaitForSeconds(waitTimeAfterMelee);
        */
        //enemy.animator.SetBool("isMeleeAttacking", false);
        yield return null;
        enemy.IdleAni();
        enemy.navMeshAgent.isStopped = false;
        enemy.navMeshAgent.speed = enemy.speed;
        enemy.navMeshAgent.stoppingDistance = 0;
        

        ExitMeleeAttack();
        //enemy.bossState = BossState.inCombat;
    }
}
