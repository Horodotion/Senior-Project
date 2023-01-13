using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[System.Serializable]
[CreateAssetMenu(menuName = "Enemy Attack Value/Jumping Attack")]
public class JumpingAttack : AttackMotion
{
    [SerializeField] GameObject hitbox;
    [SerializeField] float hitboxDuration;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float forwardSpeed = 7f;
    [SerializeField] float jumpUpSpeed = 10;
    [SerializeField] float fallDownSpeed = 20;
    [HideInInspector] float verticalSpeed;

    /*
    public override void AttackingPlayer()
    {
        Debug.Log(isFiredOnce + " 1");
        if (isFiredOnce)
        {
            Debug.Log("2");
            isFiredOnce = false;
            enemy.StartCoroutine(Jumping());
        }
    }
    */
        
    public override IEnumerator AttackingPlayer()
    {
        enemy.navMeshAgent.acceleration = 0f;
        enemy.navMeshAgent.enabled = false;
        while (true)
        {
            enemy.gameObject.transform.position = enemy.gameObject.transform.position + (enemy.gameObject.transform.up * verticalSpeed
                + enemy.gameObject.transform.forward * forwardSpeed) * Time.deltaTime;

            if (verticalSpeed < 0)
            {
                verticalSpeed -= fallDownSpeed * Time.deltaTime;
            }
            else
            {
                verticalSpeed -= jumpUpSpeed * Time.deltaTime;
            }

            if (verticalSpeed < 0 && NavMesh.SamplePosition(enemy.gameObject.transform.position, out NavMeshHit temp, 0.5f, NavMesh.AllAreas))
            {
                GameObject hb = Instantiate(hitbox, SP[0].transform.position, SP[0].transform.rotation);
                hb.GetComponent<JumpAttackHB>().SetUp(hitboxDuration, damage);
                break;
            }
            yield return null;
        }

        NavMesh.SamplePosition(enemy.gameObject.transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas);
        verticalSpeed = jumpSpeed;
        enemy.navMeshAgent.acceleration = enemy.acceleration;
        enemy.navMeshAgent.Warp(hit.position);
        enemy.navMeshAgent.enabled = true;
        isFiredOnce = true;
        enemy.bossState = BossState.testState;
        
    }
    /*
    public override void AttackingPlayer()
    {
        //StartCoroutine(Jump());

        enemy.navMeshAgent.enabled = false;
        Debug.Log(jumpSpeed);
        enemy.gameObject.transform.position = enemy.gameObject.transform.position + (enemy.gameObject.transform.up * verticalSpeed 
            + enemy.gameObject.transform.forward * forwardSpeed) * Time.deltaTime;

        if (verticalSpeed < 0)
        {
            verticalSpeed -= fallDownSpeed * Time.deltaTime;
        }
        else
        {
            verticalSpeed -= jumpUpSpeed * Time.deltaTime;
        }


        enemy.AimTowards(PlayerController.puppet.transform.position, 1f);

        if (verticalSpeed < 0 && NavMesh.SamplePosition(enemy.gameObject.transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
        {
            verticalSpeed = jumpSpeed;
            enemy.navMeshAgent.Warp(hit.position);
            enemy.navMeshAgent.enabled = true;
            enemy.enemyState = EnemyState.aggroToPlayer;
        }
    }
    */
}
