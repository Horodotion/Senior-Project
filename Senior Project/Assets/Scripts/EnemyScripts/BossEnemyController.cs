using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum BossState
{
    idle,
    testState,
    testState2,
    meleeAttack,
    rangedAttack,
    ambushed,
    taunt,
    takingCover,
    waitingInCover,
    teleportBehindPlayer,
    laserAttack
}

public class Decision : ScriptableObject
{
    //protected Decision [] decisions;
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
    [HideInInspector] AttacksManager attacksManager;
    [SerializeField] public MovementDecision ambushedDicision;
    [SerializeField] public MovementDecision [] ambushedDicisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision attackDicision;
    [SerializeField] public MovementDecision[] attackDicisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision rangedAtkFollowUpDicision;

    [SerializeField] public MovementDecision meleeAtkFollowUpDicision;

    [SerializeField] public float bossSize;

    [Header("Boss Covering System")]
    //public LayerMask coverLayers;
    public LayerMask hidingSpotLayer;
    public LayerMask ignoreLayer;
    //public Enemy_LineOfSightChecker losChecker;
    public float playerSpottedDistance;
    public float minWaitTimeinCover;
    public float maxWaitTimeInCover;

    [Tooltip("Will not seek cover at any points within this range of the player")] public float playerTooCloseDistanceToCover = 4f;

    [Range(.001f, 5)] [Tooltip("Interval in seconds for the enemy to check for new hiding spots")] public float coverUpdateFrequency = .75f;
    [HideInInspector] float test = 0;


    [SerializeField] public float coverSampleDistance;
    

    [Header("Boss Teleport System")]
    [SerializeField] public float teleportSampleDistance;
    public float playerTooCloseDistanceToTeleport = 4f;

    public BossState state = BossState.idle;
    
    private BossState tempState;
    public BossState bossState // Public boss state variable that can be set to trigger a clean state transition
    {
        get
        {
            return state;
        }
        set
        {
            tempState = state;
            state = value;
            //OnBossStateChange?.Invoke(state, value);
            OnBossStateChange?.Invoke(tempState, state);
            //state = value;
        }
    }
    public delegate void OnStateChange(BossState oldState, BossState newState);
    public OnStateChange OnBossStateChange;

    // Coroutine variables
    public IEnumerator MovementCoroutine;
    public void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        attacksManager = GetComponent<AttacksManager>();
        //HandleStateChange(state, BossState.inCombat);
        OnBossStateChange += HandleStateChange;
        //player = PlayerController.puppet;

        
    }
    void Start()
    {
        //StartCoroutine(InCombatState());
        //MovementDecision testDiscision = new MovementDecision(0,0,0,1);
        //for (int i = 0; i < 100; i++)
        //{
        //    Debug.Log(testDiscision.GiveTheNextRandomDicision());
        //}

        bossState = BossState.laserAttack;
    }
    
    public void HandleStateChange(BossState oldState, BossState newState) // Standard handler for boss states and transitions
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
            case BossState.testState:
                MovementCoroutine = TestState();
                break;
            case BossState.testState2:
                MovementCoroutine = TestAttack();
                break;
            case BossState.taunt:
                MovementCoroutine = TakeCoverState(PlayerController.puppet.transform);//Take care the taunt later
                break;
            case BossState.meleeAttack:
                MovementCoroutine = attacksManager.MeleeAttack();
                break;
            case BossState.rangedAttack:
                MovementCoroutine = attacksManager.RangedAttack();
                break;
            case BossState.takingCover:
                MovementCoroutine = TakeCoverState(PlayerController.puppet.transform);
                break;
            case BossState.waitingInCover:
                MovementCoroutine = WaitInCoverState(Random.Range(minWaitTimeinCover, maxWaitTimeInCover));
                break;
            case BossState.teleportBehindPlayer:
                MovementCoroutine = TeleportingBehindPlayer(PlayerController.puppet.transform);
                break;
            case BossState.laserAttack:
                MovementCoroutine = attacksManager.LaserAttack();
                break;
        }
        StartCoroutine(MovementCoroutine);
    }

    private void Update()
    {
        //Debug.Log("Update state: " + bossState);
    }

    public IEnumerator InTauntState()
    {
        
        yield return null;
    }
    public IEnumerator TestAttack()
    {
        yield return new WaitForSeconds(1f);
        bossState = BossState.testState;
        yield return null;
    }
    public IEnumerator TestState()
    {
        //navMeshAgent.updatePosition = false;
        //WaitForSeconds wait = new WaitForSeconds(4);
        //Debug.Log(PlayerController.puppet);
        //navMeshAgent.destination = PlayerController.puppet.transform.position;

        //WaitForSeconds wait = new WaitForSeconds(4);
        //yield return new WaitForSeconds(1f);
        //bossState = BossState.attacking;
        //yield return new WaitForSeconds(1f);
        Debug.Log(MovementCoroutine);
        //StopCoroutine(MovementCoroutine);
        Debug.Log("4 " + bossState);
        

        navMeshAgent.SetDestination(PlayerController.puppet.transform.position);
        //yield return new WaitForSeconds(1f);
        //Debug.Log("5 " + bossState);
        yield return null;
        Debug.Log("Set");
        bossState = BossState.meleeAttack;
    }

    private IEnumerator TakeCoverState(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(coverUpdateFrequency);
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverSampleDistance, hidingSpotLayer);

            if (hitColliders.Length == 0)
            {
                Debug.Log("Unable to find cover");
                //bossState = BossState.takingCover;
            }
            else
            {
                //Find the right collider to hide
                Collider tempCol = FindValidHidingSpot(target, hitColliders);
                //Debug.Log(tempCol.transform.position);
                if (tempCol == null)
                {
                    Debug.Log("No valid cover spot");
                    //bossState = BossState.takingCover;
                }
                else
                {
                    navMeshAgent.SetDestination(tempCol.transform.position);
                }
            }

            //navMeshAgent.SetDestination(hitColliders[0].transform.position);

            if (transform.position.x == navMeshAgent.destination.x && transform.position.z == navMeshAgent.destination.z)
            {
                //Debug.Log("Test3");
                bossState = BossState.waitingInCover;
            }

            yield return wait;
        }
    }

    public Collider FindValidHidingSpot(Transform target, Collider[] colliders)
    {
        Collider tempCol = null;
        foreach (Collider thisCol in colliders)
        {

            if (!IsItAValidhidingPoint(bossSize, thisCol.transform.position))
            {
                continue;
            }

            if (Vector3.Distance(target.position, thisCol.transform.position) < playerTooCloseDistanceToCover)
            {
                continue;
            }

            if (tempCol == null)
            {
                tempCol = thisCol;
            }
            else if (Vector3.Distance(thisCol.transform.position, target.position) < Vector3.Distance(tempCol.transform.position, target.position))
            {
                tempCol = thisCol;
            }
        }
        return tempCol;
    }

    //Check if a point is a valid hiding Spot. Size is the current object size. Position is the current position that is tried to hide.
    public bool IsItAValidhidingPoint(float size, Vector3 position)
    {
        Vector3 vectorToColloder = Camera.main.transform.position - position;
        Vector3 perVectorToColloder = vectorToColloder;
        perVectorToColloder.y = perVectorToColloder.x;
        perVectorToColloder.x = perVectorToColloder.z;
        perVectorToColloder.z = -perVectorToColloder.y;
        perVectorToColloder.y = 0;
        perVectorToColloder = perVectorToColloder.normalized;

        Vector3 checkForPlayerPoint1 = position + (perVectorToColloder * bossSize / 2);
        Vector3 checkForPlayerPoint2 = position - (perVectorToColloder * bossSize / 2);

        Debug.DrawRay(checkForPlayerPoint1, Camera.main.transform.position - checkForPlayerPoint1, Color.red);
        Debug.DrawRay(checkForPlayerPoint2, Camera.main.transform.position - checkForPlayerPoint2, Color.green);

        Physics.Raycast(checkForPlayerPoint1, Camera.main.transform.position - checkForPlayerPoint1, out RaycastHit hit, Mathf.Infinity, ~ignoreLayer);
        Physics.Raycast(checkForPlayerPoint2, Camera.main.transform.position - checkForPlayerPoint2, out RaycastHit hit2, Mathf.Infinity, ~ignoreLayer);


        //Check the two corner for player
        if (hit.collider != null && hit.collider.tag.Equals("Player"))
        {
            return false;
        }

        if (hit2.collider != null && hit2.collider.tag.Equals("Player"))
        {
            return false;
        }

        return true;
    }

    private IEnumerator WaitInCoverState(float secondsToWait) // Either breakWhenSpotted should be true, or secondsToWait should be >0, or both. If not this would go forever
    {
        // Check to see if this call is actually capable of ending
        if (secondsToWait == 0)
        {
            bossState = AttackDicision();
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
            Vector3 vToPlayer = PlayerController.puppet.transform.position - transform.position;
            if (Physics.Raycast(transform.position, vToPlayer, out RaycastHit hit, playerSpottedDistance, ~hidingSpotLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    bossState = AmbushedDicision();
                    //yield break;
                }
            }

            yield return wait;
            //bossState = BossState.takingCover;
            bossState = AttackDicision();
            //yield break;
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

    //This output a bossstate by calculate the bossstate using the decision and decision modifier during ambushed decision.
    public BossState AmbushedDicision()
    {
        MovementDecision temp = new MovementDecision(ambushedDicision);

        // Adding all the decision modifers into the decision
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
        // Find which bossstate to output
        return temp.GiveTheNextRandomDicision();
    }

    //This output a bossstate by calculate the bossstate using the decision and decision modifier during attack decision.
    public BossState AttackDicision()
    {
        MovementDecision temp = new MovementDecision(attackDicision);

        // Adding all the decision modifers into the decision
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
        temp.DisplayLog();
        // Find which bossstate to output
        return temp.GiveTheNextRandomDicision();
    }

    public IEnumerator TeleportingBehindPlayer(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(coverUpdateFrequency);
        Debug.Log("Test");
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, teleportSampleDistance, hidingSpotLayer);

            if (hitColliders.Length == 0)
            {
                Debug.Log("Unable to find point to teleport");
            }
            else
            {
                //Find the right collider to hide
                Collider tempCol = FindValidTeleportSpot(target, hitColliders);
                if (tempCol == null)
                {
                    Debug.Log("No valid teleport spot");
                }
                else
                {
                    this.transform.position = tempCol.transform.position;
                }
            }
            Debug.Log(hitColliders.Length);

            //navMeshAgent.SetDestination(hitColliders[0].transform.position);

            if (transform.position.x == navMeshAgent.destination.x && transform.position.z == navMeshAgent.destination.z)
            {
                //Debug.Log("Test3");
                yield return wait;
                bossState = BossState.takingCover;
            }

            yield return null;
        }
    }

    public Collider FindValidTeleportSpot(Transform target, Collider[] colliders)
    {
        Collider tempCol = null;
        foreach (Collider thisCol in colliders)
        {
            Vector3 vectorToColloder = thisCol.transform.position - target.position;
            if (Vector3.Dot(vectorToColloder, target.forward) > 0)
            {
                continue;
            }

            if (!IsItAValidhidingPoint(bossSize, thisCol.transform.position))
            {
                continue;
            }

            if (Mathf.Abs(vectorToColloder.magnitude) < playerTooCloseDistanceToCover)
            {
                continue;
            }


            if (tempCol == null)
            {
                tempCol = thisCol;
            }
            else if (Vector3.Distance(thisCol.transform.position, target.position) < Vector3.Distance(tempCol.transform.position, target.position))
            {
                tempCol = thisCol;
            }
        }
        return tempCol;
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
        Physics.Raycast(viewPoint.transform.position, veiwToPlayerMesh, out hit, range, ~hidingSpotLayer);
        if (hit.collider != null && hit.collider.tag == "Player")
        {
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
    public virtual void AimTowards(Vector3 position, float aimSpeed)
    {
        Vector3 veiwToPlayerMesh = position - viewPoint.transform.position;
        veiwToPlayerMesh.y = 0;
        transform.forward = Vector3.RotateTowards(transform.forward, veiwToPlayerMesh, aimSpeed * Time.deltaTime, 0.0f);
        Debug.DrawRay(viewPoint.transform.position, veiwToPlayerMesh, Color.blue);
    }

    public virtual void AimTowardsWithY(GameObject gameObject, Vector3 position, float aimSpeed)
    {
        Vector3 veiwToPlayerMesh = position - gameObject.transform.position;
        //veiwToPlayerMesh.x = 0;
        transform.forward = Vector3.RotateTowards(gameObject.transform.forward, veiwToPlayerMesh, aimSpeed * Time.deltaTime, 0.0f);
        Debug.DrawRay(gameObject.transform.position, veiwToPlayerMesh, Color.red);
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
