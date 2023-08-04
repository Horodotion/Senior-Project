using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Ice Projectile Attack")]
public class IceProjectileAttacks : AttackMotion
{
    [SerializeField] GameObject projectile;
    //[SerializeField] float projectileForce = 20;
    [SerializeField] float fireDistance = 15;
    [SerializeField] float aimSpeed = 0.5f;
    //[SerializeField] float waitTimeAfterFire = 0.5f;
    /*
    public IceProjectileAttacks(BossEnemyController enemyController, Transform[] SP)
    {
        //enemy = enemyController;
        this.SP = SP;
    }
    */
    public override IEnumerator AttackingPlayer(BossEnemyController enemy, int leftRightHand)
    {

        while (true)
        {
            enemy.navMeshAgent.speed = enemy.speed;
            enemy.RunningAni();
            //yield return null;
            Physics.Raycast(enemy.transform.position, PlayerController.puppet.cameraObj.transform.position - enemy.transform.position, out RaycastHit hit, fireDistance, ~LayerMask.GetMask("Enemy"));
            Debug.DrawRay(enemy.transform.position, PlayerController.puppet.cameraObj.transform.position - enemy.transform.position, Color.red);
            //Debug.Log(hit.collider.tag);
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);
            if (hit.collider != null && hit.collider.tag.Equals("Player"))
            {
                //yield return null;
                break;
            }
            yield return null;
            
        }

        enemy.animator.SetFloat(enemy.aniLeftRightDecision, leftRightHand);
        enemy.animator.SetInteger(enemy.aniDecision, enemy.throwAni);
        enemy.animator.SetFloat("element", 0);

        
        //yield return new WaitForSeconds(2f);
        enemy.navMeshAgent.isStopped = true;
        //yield return new WaitForSeconds(1f);
        while (enemy.animator.GetInteger(enemy.aniDecision) == enemy.throwAni)
        {
            
            while (!enemy.IsPlayerWithinView(100f, 4f, 100f))
            {
                enemy.AimTowards(PlayerController.puppet.transform.position, aimSpeed);
                SP[leftRightHand].LookAt(PlayerController.puppet.cameraObj.transform.position);
                yield return null;
            }

            yield return null;
        }
        /*
        while (!enemy.IsPlayerWithinView(100f, 4f, 100f))
        {
            enemy.AimTowards(PlayerController.puppet.transform.position, aimSpeed);
            yield return null;
        }
        
        //GameObject thisProjectile = Instantiate(projectile, SP[0].position, SP[0].rotation);
        //thisProjectile.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);

        GameObject thisProjectile1 = SpawnManager.instance.GetGameObject(projectile, SpawnType.projectile);
        //Debug.Log(thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController testController));
        if (thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController projectileController))
        {
            projectileController.transform.position = SP[0].transform.position;
            projectileController.transform.rotation = SP[0].transform.rotation;
            //SpawnManager.instance.GetGameObject(projectile, SpawnType.projectile);
            projectileController.LaunchProjectile();
        }

        yield return new WaitForSeconds(waitTimeAfterFire);

        */
        //enemy.animator.SetBool("isRangedAttacking", false);
        yield return null;

        enemy.navMeshAgent.isStopped = false;
        enemy.navMeshAgent.speed = enemy.speed;

        ExitRangedAttack(enemy);
        //enemy.bossState = BossState.inCombat;
        //yield return null;
    }
    public GameObject getProjectile()
    {
        return projectile;
    }
}
