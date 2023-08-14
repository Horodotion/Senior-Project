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


    public override IEnumerator AttackingPlayer(BossEnemyController enemy, int leftRightHand)
    {
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

        enemy.IdleAni();
        yield return null;

        enemy.animator.SetFloat(enemy.aniLeftRightDecision, leftRightHand);
        enemy.animator.SetInteger(enemy.aniDecision, enemy.meleeAni);
        enemy.animator.SetFloat("element", 0);

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

            if (SP[leftRightHand].gameObject.TryGetComponent<HitBoxController>(out HitBoxController hitbox))
            {
                hitbox.damage = -damage;
            }

            yield return null;
        }

        yield return null;
        enemy.navMeshAgent.isStopped = false;

        enemy.navMeshAgent.speed = enemy.speed;
        enemy.navMeshAgent.stoppingDistance = 0;

        ExitMeleeAttack(enemy);
    }

    public void SetHitBoxActive()
    {
        SP[0].gameObject.SetActive(true);
    }

    public void SetHitBoxInactive()
    {
        SP[0].gameObject.SetActive(false);
    }
}
