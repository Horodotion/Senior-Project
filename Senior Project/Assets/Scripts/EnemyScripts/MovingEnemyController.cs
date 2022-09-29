using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    idle,
    aggroToPlayer,
    lostThePlayer,
    returnToSP, //Return to spawn point
    attacking,
    other
}

public class MovingEnemyController : EnemyController
{
    

    [HideInInspector] public Animator thisAnimator;

    [HideInInspector] public NavMeshAgent navMeshAgent;

    [HideInInspector] public Vector3 spawnPoint;

    //[HideInInspector] public float stats.stat[StatType.speed]; //Will be changing this to stats

    [HideInInspector] private float timerForAfterLostPlayer;

    public EnemyState enemyState;

    [Header("Enemy Movement Setting")]

    [SerializeField] public GameObject viewPoint; // The starting point of the enemy view point
    [SerializeField] public float viewDegreeH = 100; // The Horizontal angle where the enemy can see the player
    [SerializeField] public float viewDegreeV = 50; // The Vertical angle where the enemy can see the player

    [SerializeField] public float viewRange = 10; // The distance that the enemy can see the player
    [SerializeField] float hearingRange = 3; // The distance that the enemy can hear the player
    [SerializeField] float lostPlayerRange = 15; // The distance that the enemy will lost/stop follow the player when aggro
    [SerializeField] float stoppingRange = 3; // Stop at what distance to the player when reaching to the player

    [SerializeField] float waitTimeAfterLostPlayer = 1;

    [HideInInspector] public string aniMove = " ";

    // Start is called before the first frame update
    public virtual void Awake()
    {
        
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        navMeshAgent.stoppingDistance = stoppingRange;
        spawnPoint = transform.position;
        enemyState = EnemyState.idle;
    }

    public virtual void AniParameterName()
    {

    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        stats = Instantiate(stats);
        stats.SetStats();
        switch (enemyState)
        {
            case EnemyState.idle:
                InIdleState();
                break;
            case EnemyState.aggroToPlayer:
                InAggroState();
                break;
            case EnemyState.lostThePlayer:
                InLostThePlayerState();
                break;
            case EnemyState.returnToSP:
                InReturnToSPState();
                break;
            case EnemyState.attacking:
                Attack();
                break;
        }

    }

    public virtual void InIdleState()
    {
        MoveAni(false);
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.speed = 0;
        if (IsPlayerWithinView(viewRange, viewDegreeH, viewDegreeV) || IsPlayerWithinDistance(hearingRange))
        {
            //thisAnimator.SetBool("Idle", false);
            enemyState = EnemyState.aggroToPlayer;
        }
    }

    public virtual void InAggroState()
    {
        
        if (IsPlayerWithinDistance(navMeshAgent.stoppingDistance))
        {
            MoveAni(false);
            AimTowardsPlayer();
        }
        else
        {
            MoveAni(true);
        }
        navMeshAgent.stoppingDistance = 3;
        navMeshAgent.speed = stats.stat[StatType.speed];
        navMeshAgent.destination = PlayerController.puppet.transform.position;
        if (!IsPlayerWithinDistance(lostPlayerRange))
        {
            timerForAfterLostPlayer = waitTimeAfterLostPlayer;
            enemyState = EnemyState.lostThePlayer;
        }
        Attack();
    }

    public virtual void InLostThePlayerState()
    {
        MoveAni(false);
        timerForAfterLostPlayer -= Time.deltaTime;
        navMeshAgent.speed = 0;
        if (timerForAfterLostPlayer < 0)
        {
            enemyState = EnemyState.returnToSP;
            timerForAfterLostPlayer = waitTimeAfterLostPlayer;
        }
        if (IsPlayerWithinView(viewRange, viewDegreeH, viewDegreeV) || IsPlayerWithinDistance(hearingRange))
        {
            enemyState = EnemyState.aggroToPlayer;
            timerForAfterLostPlayer = waitTimeAfterLostPlayer;
        }
    }


    public virtual void InReturnToSPState()
    {
        MoveAni(true);
        navMeshAgent.speed = stats.stat[StatType.speed];
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.destination = spawnPoint;
        if (IsPlayerWithinView(viewRange, viewDegreeH, viewDegreeV) || IsPlayerWithinDistance(hearingRange))
        {
            enemyState = EnemyState.aggroToPlayer;
        }
        if (Vector3.Distance(spawnPoint, transform.position) < 0.1)
        {
            enemyState = EnemyState.idle;
        }
    }

    public virtual void Attack()
    {

    }


    public bool IsPlayerWithinDistance(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider thisCollider in colliders)
        {
            if (thisCollider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    //This is to check if the player is within view.
    public bool IsPlayerWithinView(float range, float degreeH, float degreeV)
    {
        RaycastHit hit;
        Vector3 veiwToPlayerMesh = PlayerController.puppet.cameraObj.transform.position - viewPoint.transform.position;
        Physics.Raycast(viewPoint.transform.position, veiwToPlayerMesh, out hit, range);
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            Debug.DrawRay(viewPoint.transform.position, veiwToPlayerMesh, Color.blue);
            float angleH = Vector3.Angle(new Vector3(veiwToPlayerMesh.x, 0, veiwToPlayerMesh.z), viewPoint.transform.forward);
            float angleV = Vector3.Angle(new Vector3(viewPoint.transform.forward.x, veiwToPlayerMesh.y, viewPoint.transform.forward.z), viewPoint.transform.forward);
            if (angleH < degreeH / 2 && angleV < degreeV / 2)
            {
                Debug.DrawRay(viewPoint.transform.position, veiwToPlayerMesh, Color.blue);
                return true;
            }
        }
        return false;
    }

    public virtual void AimTowardsPlayer()
    {
    }


    public override void Damage(float damageAmount)
    {
        stats.AddToStat(StatType.health, -damageAmount);
        if (stats.stat[StatType.health] <= 0)
        {
            CommitDie();
        }
        else
        {
            if (enemyState != EnemyState.aggroToPlayer || enemyState != EnemyState.attacking)
            {
                enemyState = EnemyState.aggroToPlayer;
            }
        }
    }


    public override void CommitDie()
    {
        dead = true;
        Destroy(this.gameObject);
    }
    public virtual void MoveAni(bool isMove)
    {

    }
}
