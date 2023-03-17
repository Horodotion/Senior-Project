using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum BossState
{
    idle = 0,
    takingCover = 1,
    meleeAttack = 2,
    rangedAttack = 3,
    taunt = 4,
    teleportBehindPlayer = 5,
    waitingInCover = 6,
    laserAttack = 7,
    ambushed,

    testState,
    testState2,
    
    
    
    
    
    
}

public class Decision : ScriptableObject
{
    //protected Decision [] decisions;
}

public class BossEnemyController : EnemyController
{
    [Header("Boss Stats")]
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;

    [SerializeField] public float speed = 3.5f;
    [SerializeField] public float acceleration = 8f;
    [SerializeField] public float angularSpeed = 120f;

    
    
    [SerializeField] public GameObject viewPoint; // The starting point of the enemy view point
    [SerializeField] public float viewDegreeH = 100; // The Horizontal angle where the enemy can see the player
    [SerializeField] public float viewDegreeV = 50; // The Vertical angle where the enemy can see the player
    [SerializeField] public float viewRange = 10; // The distance that the enemy can see the player

    [Header("Boss Dicision Setting")]
    [HideInInspector] AttacksManager attacksManager;
    [SerializeField] public MovementDecision coverActionDicision;
    [SerializeField] public MovementDecision [] coverActionDicisionMod;

    [SerializeField] public MovementDecision ambushedDicision;
    [SerializeField] public MovementDecision [] ambushedDicisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision attackDicision;
    [SerializeField] public MovementDecision[] attackDicisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision rangedAtkFollowUpDicision;

    [SerializeField] public MovementDecision meleeAtkFollowUpDicision;

    

    [Header("Boss Covering System")]
    //public LayerMask coverLayers;
    [SerializeField] public float widthOfTheBoss;
    public LayerMask hidingSpotLayer;
    public LayerMask ignoreLayer;
    //public Enemy_LineOfSightChecker losChecker;
    public float playerSpottedDistance;
    public float minWaitTimeinCover;
    public float maxWaitTimeInCover;

    [Tooltip("Will not seek cover at any points within this range of the player")] public float playerTooCloseDistanceToCover = 4f;

    [Range(.001f, 5)] [Tooltip("Interval in seconds for the enemy to check for new hiding spots")] public float coverUpdateFrequency = .75f;

    [SerializeField] public float coverSampleDistance;

    private bool isCoverPointResetNeeded = true;

    private Collider nextCol;
    private Vector3 prevousCoverPoint;


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
        navMeshAgent.angularSpeed = angularSpeed;
        navMeshAgent.acceleration = acceleration;
        attacksManager = GetComponent<AttacksManager>();
        if (TryGetComponent<Animator>(out Animator thatAnimator))
        {
            animator = thatAnimator;
        }
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

        bossState = BossState.takingCover;
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
                MovementCoroutine = TakeCoverState(PlayerController.puppet.transform); //Take care the taunt later
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
                MovementCoroutine = WaitInCoverState(UnityEngine.Random.Range(minWaitTimeinCover, maxWaitTimeInCover));
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
        Ani();
        
    }
    private void Ani()
    {
        Debug.Log(navMeshAgent.speed);
        if (animator == null) return;
        if (navMeshAgent.speed > 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
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
        navMeshAgent.speed = speed;
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
                
                if (nextCol != null)
                {
                    if (isCoverPointResetNeeded)
                    {
                        //Debug.Log("Hey");
                        prevousCoverPoint = nextCol.transform.position;
                        isCoverPointResetNeeded = false;
                    }
                }
                
                //Find the right collider to hide
                nextCol = FindValidHidingSpot(target, hitColliders);
                //Debug.Log(tempCol.transform.position);
                if (nextCol == null)
                {
                    Debug.Log("No valid cover spot");
                    //bossState = BossState.takingCover;
                }
                else
                {
                    //Debug.Log(prevousCoverPoint + " "+ nextCol.transform.position);
                    
                    navMeshAgent.SetDestination(nextCol.transform.position);

                    
                    if (prevousCoverPoint != nextCol.transform.position)
                    {
                        //Debug.Log("Hey2");
                        isCoverPointResetNeeded = true;
                    }
                    
                }
            }

            //navMeshAgent.SetDestination(hitColliders[0].transform.position);

            if (transform.position.x == navMeshAgent.destination.x && transform.position.z == navMeshAgent.destination.z)
            {
                //Debug.Log("Test3");
                isCoverPointResetNeeded = true;
                bossState = BossState.waitingInCover;// CoverActionDicision();
            }

            yield return wait;
        }
    }

    public Collider FindValidHidingSpot(Transform target, Collider[] colliders)
    {
        Collider tempCol = null;
        foreach (Collider thisCol in colliders)
        {
            /*
            Vector3 playerToBossVector = transform.position - target.position;
            //Checked if it's in front of the boss 
            
            if (Vector3.Dot(playerToBossVector, target.forward) > 0)
            {
                Vector3 playerToColloderVector = thisCol.transform.position - target.position;
                if (Vector3.Dot(playerToBossVector, target.right) > 0)
                {
                    if (Vector3.Dot(playerToColloderVector, target.right) < 0)
                    {
                        continue;
                    }

                }
                else
                {
                    if (Vector3.Dot(playerToColloderVector, target.right) > 0)
                    {
                        continue;
                    }
                }
            }
            */
            if (prevousCoverPoint == thisCol.transform.position && isCoverPointResetNeeded)
            {
                continue;
            }
            


            if (!IsItAValidHidingPoint(widthOfTheBoss, thisCol.transform.position))
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
    public bool IsItAValidHidingPoint(float size, Vector3 position)
    {
        //Find the two points that is between the boss while also perpendicular to the player
        Vector3 vectorToColloder = Camera.main.transform.position - position;
        Vector3 perVectorToColloder = vectorToColloder;
        perVectorToColloder.y = perVectorToColloder.x;
        perVectorToColloder.x = perVectorToColloder.z;
        perVectorToColloder.z = -perVectorToColloder.y;
        perVectorToColloder.y = 0;
        perVectorToColloder = perVectorToColloder.normalized;

        Vector3 checkForPlayerPoint1 = position + (perVectorToColloder * widthOfTheBoss / 2);
        Vector3 checkForPlayerPoint2 = position - (perVectorToColloder * widthOfTheBoss / 2);

        Debug.DrawRay(checkForPlayerPoint1, Camera.main.transform.position - checkForPlayerPoint1, Color.red);
        Debug.DrawRay(checkForPlayerPoint2, Camera.main.transform.position - checkForPlayerPoint2, Color.green);

        //Fires raycast to check if both of the points hit a wall or the boss itself.
        Physics.Raycast(checkForPlayerPoint1, Camera.main.transform.position - checkForPlayerPoint1, out RaycastHit hit, Mathf.Infinity, ~ignoreLayer);
        Physics.Raycast(checkForPlayerPoint2, Camera.main.transform.position - checkForPlayerPoint2, out RaycastHit hit2, Mathf.Infinity, ~ignoreLayer);


        //If either one of the raycasts hit the player, return false
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
        navMeshAgent.speed = 0;
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
                    //bossState = BossState.takingCover;
                    yield break;
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
    //This output a bossstate by calculate the bossstate using the decision and decision modifier during CoverAction decision.
    public BossState CoverActionDicision()
    {
        MovementDecision temp = new MovementDecision(coverActionDicision);

        // Adding all the decision modifers into the decision
        if (!IsPlayerWithinDistance(25))
        {
            temp.AddDicision(coverActionDicisionMod[0]);
        }
        if (health.stat / health.maximum <= 0.5)
        {
            temp.AddDicision(coverActionDicisionMod[1]);
        }
        // Find which bossstate to output
        return temp.GiveTheNextRandomDicision();
    }


    //This output a bossstate by calculate the bossstate using the decision and decision modifier during ambushed decision.
    public BossState AmbushedDicision()
    {
        MovementDecision temp = new MovementDecision(ambushedDicision);

        // Adding all the decision modifers into the decision
        if (health.stat / health.maximum > 0.5)
        {
            temp.AddDicision(ambushedDicisionMod[0]);
        }
        if (health.stat / health.maximum > 0.3)
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
                // If null, then boss is unable to find a spot to teleport behind
                if (tempCol == null)
                {

                    tempCol = FindValidHidingSpot(target, hitColliders);

                    // If null, then boss is unable to find a spot to teleport
                    if (tempCol == null)
                    {
                        Debug.Log("No valid teleport spot");
                        bossState = BossState.takingCover;
                    }
                    else
                    {
                        this.transform.position = tempCol.transform.position;
                        bossState = BossState.meleeAttack;
                    }
                    //hitColliders
                }
                else
                {
                    this.transform.position = tempCol.transform.position;
                    bossState = BossState.meleeAttack;
                }
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

            if (!IsItAValidHidingPoint(widthOfTheBoss, thisCol.transform.position))
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
        gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, veiwToPlayerMesh, aimSpeed * Time.deltaTime, 0.0f);
        Debug.DrawRay(gameObject.transform.position, veiwToPlayerMesh, Color.red);
    }

    public override void CommitDie()
    {
        base.CommitDie();

        GeneralManager.instance.WinGame();
    }
}
