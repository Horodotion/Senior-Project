using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileEnemyController : MovingEnemyController
{
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject projectileSP;
    [SerializeField] float fireRate = 3;
    [SerializeField] float preFiringWaitTime = 1f;
    [SerializeField] float projectileForce = 20;
    [HideInInspector] private float timerForProjectile = 0;
    public override void Awake()
    {
        base.Awake();
        timerForProjectile = fireRate;
    }

    public override void Attack()
    {
        
        timerForProjectile -= Time.deltaTime;
        
        switch (timerForProjectile)
        {
            case float x when x > 0 && x < preFiringWaitTime:
                WaitForProjectile();
                AimTowardsPlayer();
                break;
            case float x when x <= 0:
                FireProjectile();
                ReturnToChasing();
                break;
            
        }
            
    }

    // When the animation are here, call the functions below.
    public void WaitForProjectile()
    {
        navMeshAgent.speed = 0;
    }

    public override void AimTowardsPlayer()
    {
        Vector3 veiwToPlayerMesh = PlayerController.puppet.cameraObj.transform.position - viewPoint.transform.position;
        transform.forward = Vector3.RotateTowards(transform.forward, veiwToPlayerMesh, 1f * Time.deltaTime, 0.0f);
        Debug.DrawRay(viewPoint.transform.position, veiwToPlayerMesh, Color.blue);
    }


    public void FireProjectile()
    {
        GameObject thisProjectile = Instantiate(projectile, projectileSP.transform.position, projectileSP.transform.rotation);
        thisProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * projectileForce, ForceMode.Impulse);
        timerForProjectile = fireRate;
    }

    public void ReturnToChasing()
    {
        navMeshAgent.speed = stats.stat[StatType.speed];
    }
}
