using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Laser Attack")]
public class LaserAttacks : AttackMotion
{
    [SerializeField] float firingTime = 5;
    [SerializeField] float firingDistance = 8;
    [SerializeField] float turnSpeed = 2;
    [SerializeField] float waitTimeAfterLaser = 1f;

    public override IEnumerator AttackingPlayer()
    {
        //yield return new WaitForSeconds(2f);
        //Debug.Log("5 " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;
        //Debug.Log("6 " + enemy.bossState);
        //yield return new WaitForSeconds(5f);
        while (true)
        {
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);

            if (enemy.IsPlayerWithinDistance(firingDistance))
            {
                break;
            }

            yield return null;
        }
        //enemy.bossState = BossState.inCombat;

        enemy.navMeshAgent.speed = 0f;


        //yield return new WaitForSeconds(1f);

        for (float timer = 0; true; timer += Time.deltaTime)
        {
            enemy.AimTowards(PlayerController.puppet.cameraObj.transform.position, turnSpeed);
            Vector3 veiwToPlayerMesh = PlayerController.puppet.cameraObj.transform.position - SP[0].transform.position;
            if (Physics.Raycast(SP[0].transform.position, veiwToPlayerMesh, out RaycastHit hit, Mathf.Infinity, ~enemy.hidingSpotLayer))
            {
                enemy.AimTowardsVertical(SP[0].gameObject, PlayerController.puppet.cameraObj.transform.position, turnSpeed);
                if (hit.collider != null && hit.collider.tag.Equals("Player"))
                {
                    SP[0].gameObject.SetActive(true);
                }
            }
            if (timer > firingTime)
            {   
                break;
            }
            yield return null;
        }


        SP[0].gameObject.SetActive(false);

        yield return new WaitForSeconds(waitTimeAfterLaser);
        enemy.navMeshAgent.speed = enemy.speed;
        enemy.navMeshAgent.stoppingDistance = 0;
        enemy.bossState = enemy.rangedAtkFollowUpDicision.GiveTheNextRandomDicision();
        //enemy.bossState = BossState.inCombat;
    }
}
