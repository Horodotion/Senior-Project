using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Ice Projectile Attack")]
public class IceProjectileAttacks : AttackMotion
{
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileForce = 20;

    public IceProjectileAttacks(BossEnemyController enemyController, Transform[] SP)
    {
        enemy = enemyController;
        this.SP = SP;
    }
    public override IEnumerator AttackingPlayer()
    {

        enemy.navMeshAgent.updatePosition = false;
        for(int t = 0; true; t++)
        {
            Vector3 veiwToPlayerMesh = PlayerController.puppet.cameraObj.transform.position - enemy.viewPoint.transform.position;
            enemy.transform.forward = Vector3.RotateTowards(enemy.transform.forward, veiwToPlayerMesh, 1f * Time.deltaTime, 0.0f);
            Debug.DrawRay(enemy.viewPoint.transform.position, veiwToPlayerMesh, Color.blue);
            if (t > 4)
            {
                GameObject thisProjectile = Instantiate(projectile, SP[0].position, SP[0].rotation);
                thisProjectile.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                //enemy.bossState = enemy.rangedAtkFollowUpDicision.GiveTheNextRandomDicision();
                enemy.bossState = BossState.inCombat;

            }
            yield return null;
        }
        
    }
}
