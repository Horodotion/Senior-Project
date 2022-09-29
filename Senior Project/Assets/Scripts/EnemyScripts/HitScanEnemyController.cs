using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanEnemyController : MovingEnemyController
{
    [Header("Attacks Setting")]
    [SerializeField] GameObject enemyObjectWithAnimator;
    [SerializeField] public GameObject hitScanSP;
    [SerializeField] public float fireRate = 3;
    [SerializeField] public float weaponRange = 10;
    [SerializeField] float accuracy = 1f;
    [SerializeField] float aimSpeed = 0.2f;
    [HideInInspector]  float timerForHitScan = 0;

    [HideInInspector] string aniRaiseHand;
    [HideInInspector] string aniShoot;
    //[HideInInspector] string aniIsHit;

    public override void Awake()
    {
        base.Awake();
        if (enemyObjectWithAnimator != null && enemyObjectWithAnimator.GetComponent<Animator>() != null)
        {
            thisAnimator = enemyObjectWithAnimator.GetComponent<Animator>();
            AniParameterName();
        }
        timerForHitScan = fireRate;
    }

    public override void AniParameterName()
    {
        aniMove = "Move";
        aniRaiseHand = "Raise to shoot";
        aniShoot = "Shoot";
    }

    public override void Attack()
    {
        timerForHitScan -= Time.deltaTime;
        Debug.Log(IsPlayerWithinView(viewRange, 100, 100));
        if (IsPlayerWithinDistance(viewRange))
        {
            switch (timerForHitScan)
            {
                case float x when x > 0 && x < fireRate:

                    break;
                case float x when x <= 0:
                    //thisAnimator.SetBool(aniShoot, true);
                    WaitForFire();
                    AimTowardsPlayer();
                    //FireHitScan();
                    //ReturnToChasing();
                    break;

            }
        }
    }

    // When the animation are here, call the functions below.
    public void WaitForFire()
    {
        
        navMeshAgent.speed = 0;
        enemyState = EnemyState.attacking;
        if (thisAnimator != null)
        {
            thisAnimator.SetBool(aniRaiseHand, true);
        }
    }
    public void FireHitScanAni()
    {
        if (thisAnimator != null)
        {
            thisAnimator.SetBool(aniShoot, true);
        }
    }

    public override void AimTowardsPlayer()
    {
        Vector3 veiwToPlayerMesh = PlayerController.puppet.cameraObj.transform.position - viewPoint.transform.position;
        transform.forward = Vector3.RotateTowards(transform.forward, veiwToPlayerMesh, aimSpeed * Time.deltaTime, 0.0f);
        Debug.DrawRay(viewPoint.transform.position, veiwToPlayerMesh, Color.blue);
    }
    public void FireHitScan()
    {
        RaycastHit hit;
        Vector3 direction = Accuracy(hitScanSP.transform.forward, accuracy);
        Physics.Raycast(hitScanSP.transform.position, direction, out hit, weaponRange);
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            PlayerController.puppet.Damage(stats.stat[StatType.damage]);
        }
        timerForHitScan = fireRate;
    }

    public void LowerArms()
    {
        if (thisAnimator != null)
        {
            thisAnimator.SetBool(aniRaiseHand, false);
        }
    }

    public void ReturnToChasing()
    {
        enemyState = EnemyState.aggroToPlayer;
        navMeshAgent.speed = stats.stat[StatType.speed];
    }


    public virtual Vector3 Accuracy(Vector3 forwardDirection, float variance)
    {
        variance = (100 - variance) / 1000;
        Vector3 newDirection = forwardDirection;

        newDirection.x += Random.Range(-variance, variance);
        newDirection.y += Random.Range(-variance, variance);

        return newDirection;
    }
    public override void MoveAni(bool isMove)
    {
        if (thisAnimator != null)
        {
            thisAnimator.SetBool(aniMove, isMove);
        }
    }
}
