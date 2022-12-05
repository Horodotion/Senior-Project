using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Attack Value/Fire Melee Attack")]
public class FireMeleeAttacks : AttackMotion
{
    //[SerializeField] GameObject hitBox;
    [SerializeField] float chargeSpeed;
    [SerializeField] float meleeDistance = 1.5f;
    [SerializeField] float turnSpeed = 1.5f;
    [SerializeField] float stoppingDistance = 1f;
    [SerializeField] float hitboxSpawnTime = 0.5f;


    public override IEnumerator AttackingPlayer()
    {
        //yield return new WaitForSeconds(2f);
        //Debug.Log("5 " + enemy.bossState);
        //enemy.bossState = BossState.inCombat;
        // Debug.Log("6 " + enemy.bossState);
        //yield return new WaitForSeconds(5f);
        enemy.navMeshAgent.stoppingDistance = stoppingDistance;
        while (!enemy.IsPlayerWithinDistance(meleeDistance))
        {
            enemy.navMeshAgent.speed = chargeSpeed;
            enemy.navMeshAgent.SetDestination(PlayerController.puppet.transform.position);

            yield return null;
        }
        //enemy.bossState = BossState.inCombat;

        enemy.navMeshAgent.speed = 0f;


        //yield return new WaitForSeconds(1f);

        for (float timer = 0; true; timer += Time.deltaTime)
        {
            enemy.AimTowards(PlayerController.puppet.transform.position, turnSpeed);
            if (timer > 0.2)
            {
                break;
            }
            yield return null;
        }

        SP[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(hitboxSpawnTime);
        SP[0].gameObject.SetActive(false);
        enemy.navMeshAgent.stoppingDistance = 0;
        enemy.navMeshAgent.speed = enemy.speed;
        enemy.bossState = enemy.meleeAtkFollowUpDicision.GiveTheNextRandomDicision();
        //enemy.bossState = BossState.inCombat;
    }
}
