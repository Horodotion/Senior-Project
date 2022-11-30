using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum BossState
{
    idle,
    inCombat,
    attacking,
    meleeAttack,
    rangedAttack,
    ambushed,
    taunt,
    takingCover,
    waitingInCover
}

public class Dicision : ScriptableObject
{
}

public class BossEnemyController : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] public float maxHealth = 1000;
    [SerializeField] public float health = 1000;
    [SerializeField] public float speed = 3.5f;
    [SerializeField] public float acceleration = 8f;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    //[HideInInspector] PlayerPuppet player;
    [HideInInspector] public bool dead = false;
    
    [SerializeField] public GameObject viewPoint; // The starting point of the enemy view point
    [SerializeField] public float viewDegreeH = 100; // The Horizontal angle where the enemy can see the player
    [SerializeField] public float viewDegreeV = 50; // The Vertical angle where the enemy can see the player
    [SerializeField] public float viewRange = 10; // The distance that the enemy can see the player

    [Header("Boss Dicision Setting")]
    [SerializeField] public MovementDecision ambushedDicision;
    [SerializeField] public MovementDecision [] ambushedDicisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision attackDicision;
    [SerializeField] public MovementDecision[] attackDicisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision rangedAtkFollowUpDicision;

    [SerializeField] public MovementDecision meleeAtkFollowUpDicision;

    [Header("Boss Covering System")]
    public LayerMask coverLayers;
    public Enemy_LineOfSightChecker losChecker;
    public AttacksManager attacksManager;

    [Range(-1, 1)] [Tooltip("The further from zero, the more covered a hiding spot will have to be to get accepted")] public float coverSensitivity = -.3f;
    [Range(0, 5)] [Tooltip("The minimum height of an object for it to be considered a hiding spot")] public float minObstacleHeight = 1f;
    [Tooltip("Will not seek cover at any points within this range of the player")] public float playerTooCloseDistance = 4f;

    private Collider[] coverColliders = new Collider[20]; // How many potential hiding spots to seek out per cover cycle. This should be twice the number of objects you want to use as cover, since each object generates two potential spots.
    [Range(.001f, 5)] [Tooltip("Interval in seconds for the enemy to check for new hiding spots")] public float coverUpdateFrequency = .75f;
    [HideInInspector] float timer = 0;


    public BossState state = BossState.idle;

    public BossState bossState // Public boss state variable that can be set to trigger a clean state transition
    {
        get
        {
            return state;
        }
        set
        {
            
            OnBossStateChange?.Invoke(state, value);
            state = value;
        }
    }
    public delegate void OnStateChange(BossState oldState, BossState newState);
    public OnStateChange OnBossStateChange;

    // Coroutine variables
    public Coroutine MovementCoroutine;
    public void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        attacksManager = GetComponent<AttacksManager>();
        //HandleStateChange(state, BossState.inCombat);
        OnBossStateChange += HandleStateChange;
        //
        //Debug.Log(bossState);
        //player = PlayerController.puppet;

        
    }
    void Start()
    {
        //StartCoroutine(InCombatState());
        bossState = BossState.inCombat;
        MovementDecision testDiscision = new MovementDecision(0,0,0,1);
        //for (int i = 0; i < 100; i++)
        //{
        //    Debug.Log(testDiscision.GiveTheNextRandomDicision());
        //}
    }
    
    public void HandleStateChange(BossState oldState, BossState newState) // Standard handler for boss states and transitions
    {
        if (oldState != newState)
        {
            if (MovementCoroutine != null)
            {
                StopCoroutine(MovementCoroutine);
            }
            switch (newState)
            {
                // Spawnin probably should not be assigned with this, but putting it here just in case
                case BossState.idle:
                    break;
                case BossState.inCombat:
                    MovementCoroutine = StartCoroutine(InCombatState());
                    break;
                case BossState.attacking:
                    attacksManager.Attack();
                    break;
                case BossState.taunt:
                    //MovementCoroutine = StartCoroutine(InTauntState());
                    bossState = BossState.takingCover; //Take care the taunt later
                    break;
                case BossState.meleeAttack:
                    attacksManager.MeleeAttack();
                    break;
                case BossState.rangedAttack:
                    attacksManager.RangedAttack();
                    break;
                case BossState.takingCover:
                    MovementCoroutine = StartCoroutine(TakeCoverState(PlayerController.puppet.transform, true));
                    break;
                case BossState.waitingInCover:
                    MovementCoroutine = StartCoroutine(WaitInCoverState(Random.Range(1, 4), false));
                    break;
            }
        }
    }

    public IEnumerator InTauntState()
    {
        
        yield return null;
    }

    public IEnumerator InCombatState()
    {
        //navMeshAgent.updatePosition = false;
        //WaitForSeconds wait = new WaitForSeconds(4);
        //Debug.Log(PlayerController.puppet);
        //navMeshAgent.destination = PlayerController.puppet.transform.position;
        for (float timer = 0; true; timer += Time.deltaTime)
        {
            navMeshAgent.destination = PlayerController.puppet.transform.position;
            if (timer > 4)
            {
                bossState = BossState.attacking;
            }
            yield return null;
        }
    }

    private IEnumerator TakeCoverState(Transform target, bool stopAfterCoverFound = true)
    {
        Debug.Log("Test");
        WaitForSeconds wait = new WaitForSeconds(coverUpdateFrequency);

        while (true)
        {
            // Clear previous cover spots
            for (int i = 0; i < coverColliders.Length; i++)
            {
                coverColliders[i] = null;
            }

            // Check to see if boss has line of sight to player, show red line in inspector if not and green line if so
            Vector3 direction = (PlayerController.puppet.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out RaycastHit losHit, losChecker.sphereCollider.radius, losChecker.losLayers))
            {
                if (losHit.collider.CompareTag("Player"))
                {
                    Debug.DrawRay(transform.position, direction * losChecker.sphereCollider.radius, Color.green, .5f);
                }
                else
                {
                    Debug.DrawRay(transform.position, direction * losChecker.sphereCollider.radius, Color.red, .5f);
                }
            }

            // Find potential objects to take cover behind
            int hits = Physics.OverlapSphereNonAlloc(navMeshAgent.transform.position, losChecker.sphereCollider.radius, coverColliders, coverLayers);

            // Remove ineligible cover spots from the array
            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                // Invalidate any cover spots that are too close to the player, or are too short to hide behind, even if otherwise acceptable
                if (Vector3.Distance(coverColliders[i].transform.position, target.position) < playerTooCloseDistance || coverColliders[i].bounds.size.y < minObstacleHeight)
                {
                    coverColliders[i] = null;
                    hitReduction++;
                }
                // Check if a potential cover spot has line of sight to the player, and eliminate it if so
                else if (Physics.Raycast(coverColliders[i].transform.position, target.position, out RaycastHit spotHit, losChecker.sphereCollider.radius, losChecker.losLayers))
                {
                    if (spotHit.collider.CompareTag("Player"))
                    {
                        coverColliders[i] = null;
                        hitReduction++;
                    }
                }

            }
            hits -= hitReduction;


            // Sort array of hiding spots by distance, and shift invalid (null) results to the end
            System.Array.Sort(coverColliders, CoverColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(coverColliders[i].transform.position, out NavMeshHit hit, 2f, navMeshAgent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, navMeshAgent.areaMask))
                    {
                        Debug.LogError("Unable to find edge close to " + hit.position);
                    }

                    if (Vector3.Dot(hit.normal, (target.position - hit.position).normalized) < coverSensitivity)
                    {
                        navMeshAgent.SetDestination(hit.position);
                        //Debug.Log($"{this} moving to {hit.position}");
                        break;
                    }

                    else
                    {
                        // This checks a spot in a direction further away from the player if the previous hit location was not suitable, now trying the other side of the object
                        if (NavMesh.SamplePosition(coverColliders[i].transform.position - (target.position - hit.position).normalized * 5, out NavMeshHit hit2, 2f, navMeshAgent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, navMeshAgent.areaMask))
                            {
                                Debug.LogError("Unable to find edge close to " + hit2.position + " (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (target.position - hit2.position).normalized) < coverSensitivity)
                            {
                                navMeshAgent.SetDestination(hit2.position);
                                //Debug.Log($"{this} moving to {hit.position}");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Unable to find NavMesh near object {coverColliders[i]} at {coverColliders[i].transform.position}");
                }
            }

            // If the boss is set to stop taking cover after reaching its intended position, it moves to the waiting in cover state
            if (stopAfterCoverFound)
            {
                if (transform.position.x == navMeshAgent.destination.x && transform.position.z == navMeshAgent.destination.z)
                {
                    bossState = BossState.takingCover;
                }
            }

            yield return wait;
        }
    }

    private IEnumerator WaitInCoverState(float secondsToWait = 0, bool breakWhenSpotted = true) // Either breakWhenSpotted should be true, or secondsToWait should be >0, or both. If not this would go forever
    {
        // Check to see if this call is actually capable of ending
        if (secondsToWait == 0 && !breakWhenSpotted)
        {
            Debug.LogError("WaitInCover called with indefinite wait, returning to cover-taking state");
            bossState = BossState.takingCover;
            yield break;
        }

        // Assign how long to wait before breaking, if more than 0
        WaitForSeconds wait = null;
        if (secondsToWait > 0)
        {
            wait = new WaitForSeconds(secondsToWait);
        }

        while (true)
        {
            if (breakWhenSpotted)
            {
                // Check to see if boss has line of sight to player, and break if so
                Vector3 direction = (PlayerController.puppet.transform.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, direction, out RaycastHit losHit, losChecker.sphereCollider.radius, losChecker.losLayers))
                {
                    if (losHit.collider.CompareTag("Player"))
                    {
                        // INSERT: Whatever should happen once gaining LOS to player. This should be an ambushed reaction if the player finds it fast enough, and the queued action if the boss is ready for them
                        // BossState = BossState.Ambush; or something
                        bossState = AmbushedDicision();
                        yield break;
                    }
                }
            }

            yield return wait;
            bossState = AttackDicision();
            yield break;
        }
    }

    public int CoverColliderArraySortComparer(Collider A, Collider B) // Refer to documentation on System.Array.Sort
    {
        if (A == null && B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(navMeshAgent.transform.position, A.transform.position).CompareTo(Vector3.Distance(navMeshAgent.transform.position, B.transform.position));
        }
    }

    public BossState AmbushedDicision()
    {
        MovementDecision temp = ambushedDicision;
        if (health / maxHealth > 0.5)
        {
            temp.AddDicision(ambushedDicisionMod[0]);
        }
        if (health / maxHealth > 0.3)
        {
            temp.AddDicision(ambushedDicisionMod[1]);
        }
        if (IsPlayerWithinDistance(5))
        {
            temp.AddDicision(ambushedDicisionMod[2]);
        }
        if (!IsPlayerWithinDistance(25))
        {
            temp.AddDicision(ambushedDicisionMod[3]);
        }
        return temp.GiveTheNextRandomDicision();
    }

    public BossState AttackDicision()
    {
        MovementDecision temp = attackDicision;
        if (!IsPlayerWithinDistance(50))
        {
            temp.AddDicision(attackDicisionMod[0]);
        }
        if (!IsPlayerWithinDistance(25))
        {
            temp.AddDicision(attackDicisionMod[1]);
        }
        if (IsPlayerWithinDistance(25))
        {
            temp.AddDicision(attackDicisionMod[2]);
        }
        if (IsPlayerWithinDistance(10))
        {
            temp.AddDicision(attackDicisionMod[3]);
        }
        return temp.GiveTheNextRandomDicision();
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

    public void Damage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            CommitDie();
        }
        else
        {
            /*
            if ((enemyState != EnemyState.aggroToPlayer || enemyState != EnemyState.attacking) && enemyState != EnemyState.jumping)
            {
                //Debug.Log(enemyState != EnemyState.jumping);
                enemyState = EnemyState.aggroToPlayer;
            }
            */
        }
    }


    public void CommitDie()
    {
        dead = true;
        Destroy(this.gameObject);
    }
}
