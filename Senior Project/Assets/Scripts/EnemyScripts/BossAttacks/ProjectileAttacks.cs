using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Jumping Attack")]
public class ProjectileAttacks : EnemyAttacks
{
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileForce = 20;

    public ProjectileAttacks(BossEnemyController enemyController, Transform[] SP)
    {
        enemy = enemyController;
        this.SP = SP;
    }
    public override IEnumerator AttackingPlayer()
    {

        enemy.navMeshAgent.updatePosition = false;
        for(int t = 0; true; t++)
        {
            if (t > 4)
            {
                GameObject thisProjectile = Instantiate(projectile, SP[0].position, SP[0].rotation);
                thisProjectile.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                
            }
            yield return null;
        }
        
    }
}
