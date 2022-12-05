using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Ice Projectile Attack")]
public class IceProjectileAttacks : AttackMotion
{
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileForce = 20;
    [SerializeField] float fireDistance = 15;
    [SerializeField] float aimSpeed = 0.5f;
    [SerializeField] float waitTimeAfterFire = 0.5f;

    public IceProjectileAttacks(BossEnemyController enemyController, Transform[] SP)
    {
        enemy = enemyController;
        this.SP = SP;
    }
    public override IEnumerator AttackingPlayer()
    {
        //yield return new WaitForSeconds(2f);
        //Debug.Log("5 " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;
        // Debug.Log("6 " + enemy.bossState);
        //yield return new WaitForSeconds(5f);
        Debug.Log("Walking towards player with ice ranged attack");

        while (!enemy.IsPlayerWithinDistance(fireDistance))
        {
            Debug.Log("Am I stuck?");
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);
            
            yield return null;
        }
        Debug.Log("Firing ice attack " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;

        enemy.navMeshAgent.speed = 0f;


        //yield return new WaitForSeconds(1f);


        while (!enemy.IsPlayerWithinView(100f, 4f, 100f))
        {
            enemy.AimTowards(PlayerController.puppet.transform.position, aimSpeed);
            yield return null;
        }
        /*
        for (float timer = 0; true; timer += Time.deltaTime)
        {
            enemy.AimTowards(PlayerController.puppet.transform.position, aimSpeed);
            if (timer > 2)
            {
                break;
            }
            yield return null;
        }
        */
        
        GameObject thisProjectile = Instantiate(projectile, SP[0].position, SP[0].rotation);
        thisProjectile.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);

        yield return new WaitForSeconds(waitTimeAfterFire);
        enemy.navMeshAgent.speed = enemy.speed;
        enemy.bossState = enemy.rangedAtkFollowUpDicision.GiveTheNextRandomDicision();
        //enemy.bossState = BossState.inCombat;
        //yield return null;
    }
}
