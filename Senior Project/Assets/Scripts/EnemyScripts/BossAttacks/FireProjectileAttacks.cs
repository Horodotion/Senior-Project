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
        //RaycastHit hit;
        /*
        Physics.Raycast(enemy.transform.position, PlayerController.puppet.transform.position - enemy.transform.position, out hit, Mathf.Infinity, ~enemy.hidingSpotLayer);
        Debug.Log(hit.collider.tag);
        Debug.DrawRay(enemy.transform.position, PlayerController.puppet.transform.position - enemy.transform.position, Color.red);
        while (!enemy.IsPlayerWithinDistance(fireDistance) || hit.collider.tag != "Player")
        {
            Physics.Raycast(enemy.transform.position, PlayerController.puppet.transform.position - enemy.transform.position, out  hit, Mathf.Infinity, ~enemy.hidingSpotLayer);
            //Debug.Log("Am I stuck?");
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);

            yield return null;
        }
        */
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
        Debug.Log("Firing fire attack " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;

        enemy.navMeshAgent.speed = 0f;

        //yield return new WaitForSeconds(1f);
        
        while (true)
        {
            enemy.AimTowards(PlayerController.puppet.transform.position, aimSpeed);
            if (enemy.IsPlayerWithinView(100f, 4f, 100f))
            {

                GameObject thisProjectile1 = SpawnManager.instance.GetGameObject(projectile, SpawnType.projectile);
                Debug.Log(projectile.TryGetComponent<ProjectileController>(out ProjectileController testController));
                if (thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController projectileController))
                {
                    projectileController.origin = SP[0].transform.position;
                    projectileController.rotation = SP[0].transform.rotation;
                    //SpawnManager.instance.GetGameObject(thisProjectile1, SpawnType.projectile);
                    projectileController.LaunchProjectile();
                }
               
                //Instantiate(projectile, SP[0].position, SP[0].rotation);
                //thisProjectile1.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                /*
                yield return new WaitForSeconds(0.5f);
                GameObject thisProjectile2 = Instantiate(projectile, SP[0].position, SP[0].rotation);
                thisProjectile2.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                yield return new WaitForSeconds(0.5f);
                GameObject thisProjectile3 = Instantiate(projectile, SP[0].position, SP[0].rotation);
                thisProjectile3.GetComponent<Rigidbody>().AddForce(SP[0].transform.forward * projectileForce, ForceMode.Impulse);
                */
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
