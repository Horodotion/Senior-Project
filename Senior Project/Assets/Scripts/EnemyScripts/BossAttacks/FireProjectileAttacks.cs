using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Fire Projectile Attack")]
public class FireProjectileAttacks : AttackMotion
{
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileForce = 20;
    [SerializeField] float fireDistance = 1;
    [SerializeField] float aimSpeed = 0.5f;
    [SerializeField] float waitTimeAfterFire = 0.5f;

    public FireProjectileAttacks(BossEnemyController enemyController, Transform[] SP)
    {
        enemy = enemyController;
        this.SP = SP;
    }
    public override IEnumerator AttackingPlayer()
    {

        enemy.navMeshAgent.speed = enemy.speed;
        while (true)
        {

            Physics.Raycast(enemy.transform.position, PlayerController.puppet.cameraObj.transform.position - enemy.transform.position, out RaycastHit hit, fireDistance, ~LayerMask.GetMask("Enemy"));
            Debug.DrawRay(enemy.transform.position, PlayerController.puppet.cameraObj.transform.position - enemy.transform.position, Color.red);
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);
            if (hit.collider != null && hit.collider.tag.Equals("Player"))
            {
                break;
            }
            yield return null;
        }


        enemy.animator.SetBool("isRangedAttacking", true);
        enemy.animator.SetFloat("element", 1);


        yield return null;
        enemy.navMeshAgent.isStopped = true;
        //yield return new WaitForSeconds(1f);
        while (enemy.animator.GetBool("isRangedAttacking"))
        {

            while (!enemy.IsPlayerWithinView(100f, 4f, 100f))
            {
                enemy.AimTowards(PlayerController.puppet.transform.position, aimSpeed);
                yield return null;
            }

            yield return null;
        }

        enemy.navMeshAgent.isStopped = false;
        enemy.navMeshAgent.speed = enemy.speed;
        enemy.bossState = enemy.rangedAtkFollowUpDicision.GiveTheNextRandomDicision();
        //enemy.bossState = BossState.inCombat;
        //yield return null;
    }

    public GameObject getProjectile()
    {
        return projectile;
    }
}
