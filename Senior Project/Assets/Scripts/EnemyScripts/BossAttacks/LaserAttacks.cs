using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Laser Attack")]
public class LaserAttacks : AttackMotion
{
    [SerializeField] float laserFrequency = 0.2f;
    [SerializeField] float firingTime = 5;
    [SerializeField] float firingDistance = 8;
    [SerializeField] float turnSpeed = 2;
    [SerializeField] float waitTimeAfterLaser = 1f;
    [SerializeField] float chargeLaserTime = 5f;
    [Range(0, 2)][SerializeField] float aimLowerOnPlayerDistance = 0f;

    public override IEnumerator AttackingPlayer()
    {

        //Run Towards the player
        enemy.navMeshAgent.speed = enemy.speed;
        while (true)
        {
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);
            enemy.RunningAni();
            if (enemy.IsPlayerWithinDistance(firingDistance))
            {
                break;
            }

            yield return null;
        }

        // Start Charging laser animation
        enemy.navMeshAgent.speed = 0f;
        enemy.animator.SetInteger(enemy.aniDecision, enemy.laserAni);

        enemy.animator.SetInteger(enemy.aniLaserState, 1);

        // Laser Charging animation
        for (float timer = 0; true; timer += Time.deltaTime)
        {
            if (timer > chargeLaserTime)
            {
                enemy.animator.SetInteger(enemy.aniLaserState, 2);
                break;
            }
            yield return null;
        }

        //Aim at the player
        for (float timer = 0; true; timer += Time.deltaTime)
        {
            enemy.AimTowards(PlayerController.puppet.cameraObj.transform.position, turnSpeed);
            /*
            Vector3 veiwToPlayerMesh = PlayerController.puppet.cameraObj.transform.position - SP[0].transform.position;
            if (Physics.Raycast(SP[0].transform.position, veiwToPlayerMesh, out RaycastHit hit, Mathf.Infinity, ~enemy.hidingSpotLayer))
            {
                enemy.AimTowardsWithY(SP[0].gameObject, PlayerController.puppet.cameraObj.transform.position + new Vector3(0, -0.4f, 0), turnSpeed);
                if (hit.collider != null && hit.collider.tag.Equals("Player"))
                {
                    //Fire the laser
                    enemy.animator.SetInteger(enemy.aniLaserState, 3);
                    SP[0].gameObject.SetActive(true);
                    if (SP[0].gameObject.TryGetComponent<LaserVer2>(out LaserVer2 laser))
                    {
                        laser.laserDamageFrequency = laserFrequency;
                        laser.laserDamage = damage;
                    }
                }
            }
            */
            enemy.AimTowardsWithY(SP[0].gameObject, PlayerController.puppet.cameraObj.transform.position + new Vector3(0, -aimLowerOnPlayerDistance, 0), turnSpeed);
            enemy.animator.SetInteger(enemy.aniLaserState, 3);
            SP[0].gameObject.SetActive(true);
            if (SP[0].gameObject.TryGetComponent<LaserVer2>(out LaserVer2 laser))
            {
                laser.laserDamageFrequency = laserFrequency;
                laser.laserDamage = damage;
            }
            if (timer > firingTime)
            {   
                break;
            }
            yield return null;

        }

        enemy.animator.SetInteger(enemy.aniDecision, 0);
        enemy.animator.SetInteger(enemy.aniLaserState, 0);
        SP[0].gameObject.SetActive(false);

        yield return new WaitForSeconds(waitTimeAfterLaser);
        enemy.navMeshAgent.speed = enemy.speed;
        enemy.navMeshAgent.stoppingDistance = 0;
        
        //enemy.bossState = BossState.inCombat;

        ExitLaserAttack();
    }
}
