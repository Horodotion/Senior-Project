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
        while (!enemy.IsPlayerWithinDistance(fireDistance))
        {
            Debug.Log("Am I stuck?");
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);

            yield return null;
        }
        Debug.Log("Firing fire attack " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;

        enemy.navMeshAgent.speed = 0f;

        //yield return new WaitForSeconds(1f);

        while(true)
        {
            enemy.AimTowards(PlayerController.puppet.transform.position, aimSpeed);
            if (enemy.IsPlayerWithinView(100f, 4f, 100f))
            {
                GameObject thisProjectile1 = Instantiate(projectile, SP[0].position, SP[0].rotation);
                thisProjectile1.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                yield return new WaitForSeconds(0.5f);
                GameObject thisProjectile2 = Instantiate(projectile, SP[0].position, SP[0].rotation);
                thisProjectile2.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                yield return new WaitForSeconds(0.5f);
                GameObject thisProjectile3 = Instantiate(projectile, SP[0].position, SP[0].rotation);
                thisProjectile3.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(waitTimeAfterFire);
        enemy.navMeshAgent.speed = enemy.speed;
        enemy.bossState = enemy.rangedAtkFollowUpDicision.GiveTheNextRandomDicision();
        //enemy.bossState = BossState.inCombat;
        //yield return null;
    }
}
