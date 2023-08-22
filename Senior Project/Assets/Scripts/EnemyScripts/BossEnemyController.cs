//using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using UnityEngine.UI;
using System.Collections.Generic;

public enum BossState
{
    idle = 0,
    taunt = 1,
    meleeAttack = 2,
    rangedAttack = 3,
    takingCover = 4,
    waitingInCover = 5,
    teleportToCover = 6,
    teleportBehindPlayer = 7,
    laserAttack = 8,
    spawnTurrets = 9,
    spawnMines = 10,
    orbWalking = 11,
    spawnMobs = 12,
    ambushed = 13,
    dead = 14,
    setArmor = 15
    

    //testState,
    // testState2,



}
/*
public enum BossPhase
{
    getArmor,
    Scene1Phase1,
    Scene1Phase2,
    Scene3Phase1,
    Scene3Phase2,
    Scene3Phase3

}
*/
public class Decision : ScriptableObject
{
    //protected Decision [] decisions;
}


public class BossEnemyController : EnemyController
{
    //[SerializeField] private GameObject healthBar;
    //public NodeDecision nodeDecision;

    //public int bossPhase;

    //Health Bar system
    [SerializeField] private GameObject healthBarCanvasObject;
    [SerializeField] private Slider healthBar;

    //Boss activation system
    private bool isPlayerReachingBoss;
    [SerializeField] private float bossActivateDistance;


    // Boss mob spawn system
    [SerializeField] private GameObject mobSpawner;
    private MobSpawnerController mobSpawnerController;

    

    //[SerializeField] private BossSpawnerController turretSpawner;
    [Header("Boss Stats")]
    public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] protected AttacksManager attacksManager;

    [SerializeField] public float speed = 3.5f;
    [SerializeField] public float acceleration = 8f;
    [SerializeField] public float angularSpeed = 120f;
    
    
    [SerializeField] public GameObject viewPoint; // The starting point of the enemy view point
    [SerializeField] public float viewDegreeH = 100; // The Horizontal angle where the enemy can see the player
    [SerializeField] public float viewDegreeV = 50; // The Vertical angle where the enemy can see the player
    [SerializeField] public float viewRange = 10; // The distance that the enemy can see the player


    [Header("Boss Phase Setting")]

    public MovementPhase[] movementPhase;
    private MovementPhase phase;
    [HideInInspector]
    public MovementPhase currentMovementPhase
    {
        get
        {
            return phase;
        }
        set
        {
            if (phase != value)
            {
                if (value.isArmored)
                {
                    currentArmorHealth.stat = value.armorStats.stat;
                    currentArmorHealth.maximum = value.armorStats.maximum;
                    currentArmorHealth.minimum = value.armorStats.minimum;

                    currenthPReductionPercentOnArmorBreak = value.hPReductionPercentOnArmorBreak;
                    SetArmor(value.startingArmorType);
                }
                else
                {
                    SetArmor(DamageType.nuetral);
                }
            }
            phase = value;
        }
    }

    [Header("Armor Syatem")]
    //public DamageType startingArmorType;
    public DamageType currentArmorElementType;
    //public DamageType armorType;

    public GameObject iceArmorObject;
    public GameObject fireArmorObject;
    public IndividualStat currentArmorHealth;
    public float currenthPReductionPercentOnArmorBreak;

    public GameObject iceArmorFormedVFX;
    public GameObject fireArmorFormedVFX;

    public GameObject iceArmorDestroyedVFX;
    public GameObject fireArmorDestroyedVFX;

    /*
    [Header("Boss Dicision Setting")]
    [SerializeField] public MovementDecision coverActionDecision;
    [SerializeField] public MovementDecision [] coverActionDecisionMod;

    [SerializeField] public MovementDecision ambushedDecision;
    [SerializeField] public MovementDecision [] ambushedDecisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision attackDecision;
    [SerializeField] public MovementDecision[] attackDecisionMod = new MovementDecision[4];

    [SerializeField] public MovementDecision rangedAtkFollowUpDecision;

    [SerializeField] public MovementDecision meleeAtkFollowUpDecision;
    */

    [Header("Orbwalk System")]

    public float orbWalkSpeed;
    public float orbWalkAcceleration;
    public float turnSpeed;
    private Quaternion rotateGoalWithOutY;
    private Vector3 destination;

    [Range(0.1f, 10)] public float navMeshDetactionRadius;
    [Range(0.1f, 10)] public float navMeshDetactionDistance;

    //public float lostPlayerRange;
    public float closestStoppingDistance;
    public float farthestStoppingDistance;
    //public float maxChangeDirectionTime;
    public float minEndOrbWalkingTime;
    public float maxEndOrbWalkingTime;
    private float orbWalkingTime;

    private float endOrbWalkingTimer;
    public Vector3 moveForwardVector;
    public Vector3 moveMidRangeVector;
    public Vector3 moveBackwardVector;
    public Vector3 moveVector;


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
    public GameObject teleportVFX;
    [SerializeField] public float teleportSampleDistance;
    public float playerTooCloseDistanceToTeleport = 4f;

    [Header("Animation Setting")]
    [Range(.5f, 5)] public float orbWalkAniSpeed;
    [Range(.5f, 5)] public float runAniSpeed;
    public bool isAbleToPlayDeathAni;
    public float activationTimeAfterDeath;

    [Header("SFX")]
    public AudioSource ourAudioSource;
    public AudioClip meleeBugle;
    public AudioClip projectileBugle;
    public AudioClip laserBugle;
    public AudioClip tauntBugle;
    public AudioClip teleportBugle;


    //Animation Parameter
    [HideInInspector] public string aniDecision = "idleDecision";
    [HideInInspector] public int idleAni , runningAni, walkAni, tauntAni, throwAni, meleeAni, laserAni;

    [HideInInspector] public string aniLeftRightDecision;
    [HideInInspector] public string aniForwardBackDecision;
    [HideInInspector] public string aniElementDecision;
    [HideInInspector] public string aniLaserState;
    [HideInInspector] public string aniDeathDecision;
    //[HideInInspector] public bool isDeadAni;

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
    public virtual void Awake()
    {
        
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;
        navMeshAgent.acceleration = acceleration;
        attacksManager = GetComponent<AttacksManager>();
        mobSpawnerController = mobSpawner.GetComponent<MobSpawnerController>();
        isPlayerReachingBoss = true;
        //healthBar = healthBarCanvasObject.GetComponentInChildren<Slider>();
        if (TryGetComponent<Animator>(out Animator thatAnimator))
        {
            animator = thatAnimator;
        }
        //HandleStateChange(state, BossState.inCombat);
        OnBossStateChange += HandleStateChange;
        //player = PlayerController.puppet;

        AnimationParameter();
    }

    //public virtual void EneterNextPhase()
    //{
        //bossPhase++;
    //}

    public void AnimationParameter()
    {
        //Animation Uses
        aniDecision = "idleDecision";
        idleAni = 0;
        runningAni = 1;
        walkAni = 2;
        tauntAni = 3;
        throwAni = 4;
        meleeAni = 5;
        laserAni = 6;

        aniLeftRightDecision = "LeftRight";
        aniForwardBackDecision = "ForwardBackward";
        aniElementDecision = "element";
        aniLaserState = "laserState";
        aniDeathDecision = "isDead";
        //isDeadAni = false;
    }

    public void SpawnVFX(GameObject VFX)
    {
        GameObject VFXGameObject = SpawnManager.instance.GetGameObject(VFX, SpawnType.vfx);
        VFXGameObject.transform.position = this.transform.position;
        VFXGameObject.transform.rotation = this.transform.rotation;
        if (VFXGameObject.TryGetComponent<VisualEffect>(out VisualEffect playVFX))
        {
            playVFX.Play();
        }
    }

    public virtual void Start()
    {

        
        //EnterFireArmorState();
        ResetHealthBar();
        bossState = BossState.idle;

        //nodeDecision.InitialiseNodeDecision();
        //bossState = nodeDecision.GetCurrentState();

        //bossState = BossState.meleeAttack;
        currentMovementPhase = movementPhase[0];
        //bossState = BossState.spawnTurrets;
        //animator.SetBool(aniDeathDecision, true);
        //bossState = BossState.laserAttack;
        //bossState = BossState.taunt;
    }

    public void ResetHealthBar()
    {
        healthBar.maxValue = health.maximum;
        healthBar.minValue = health.minimum;
        healthBar.value = health.stat;
    }

    public virtual void FixedUpdate()
    {
        
        if (IsPlayerWithinDistance(bossActivateDistance) && isPlayerReachingBoss)
        {
            bossState = BossState.meleeAttack;
            //EneterNextPhase();
            isPlayerReachingBoss = false;
        }
        
        BossStageInteraction();
        //BossStageHandler();

    }
    /*
    public void BossStageHandler()
    {
        switch (bossPhase)
        {
            case BossPhase.Scene1Phase1:
                currentMovementPhase = movementPhase[0];
                break;
            case BossPhase.Scene1Phase2:
                currentMovementPhase = movementPhase[1];
                break;
            case BossPhase.Scene3Phase1:
                currentMovementPhase = movementPhase[2];
                break;
            case BossPhase.Scene3Phase2:
                currentMovementPhase = movementPhase[3];
                break;
            case BossPhase.Scene3Phase3:
                currentMovementPhase = movementPhase[4];
                break;
            case BossPhase.getArmor:
                break;
        }
    }
    */
    public void BossStageInteraction()
    {
        float healthPercentage = ((health.stat - health.minimum) / health.maximum) * 100;


        for (int i = movementPhase.Length - 1; i >= 0 ; i--)
        {
            if (healthPercentage < movementPhase[i].hPPercentageToEnterPhase)
            {
                currentMovementPhase = movementPhase[i];
                break;
            }
        }

        /*
        if (healthPercentage > 95)
        {
            bossPhase = BossPhase.Scene1Phase1;
        } 
        else if (healthPercentage > 75)
        {
            bossPhase = BossPhase.Scene1Phase2;
        } 
        else if (healthPercentage > 55)
        {
            bossPhase = BossPhase.Scene3Phase1;
        }
        else if (healthPercentage > 50)
        {
            bossPhase = BossPhase.Scene3Phase2;
        }
        else
        {
            bossPhase = BossPhase.Scene3Phase3;
        }
        */
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
            case BossState.taunt:
                //ourAudioSource.clip = tauntBugle;
                //ourAudioSource.Play();
                MovementCoroutine = TauntState(); //Take care the taunt later
                break;
            case BossState.meleeAttack:
                //ourAudioSource.clip = meleeBugle;
                //ourAudioSource.Play();
                MovementCoroutine = attacksManager.MeleeAttack();
                break;
            case BossState.rangedAttack:
                //ourAudioSource.clip = projectileBugle;
                //ourAudioSource.Play();
                MovementCoroutine = attacksManager.RangedAttack();
                break;
            case BossState.takingCover:
                MovementCoroutine = TakeCoverState(PlayerController.puppet.transform);
                break;
            case BossState.waitingInCover:
                MovementCoroutine = WaitInCoverState(UnityEngine.Random.Range(minWaitTimeinCover, maxWaitTimeInCover));
                break;
            case BossState.teleportToCover:
                //ourAudioSource.clip = teleportBugle;
                //ourAudioSource.Play();
                MovementCoroutine = TeleportingToCoverState(PlayerController.puppet.transform);
                break;
            case BossState.teleportBehindPlayer:
                //ourAudioSource.clip = teleportBugle;
                //ourAudioSource.Play();
                MovementCoroutine = TeleportingBehindState(PlayerController.puppet.transform);
                break;
            case BossState.laserAttack:
                //ourAudioSource.clip = laserBugle;
                //ourAudioSource.Play();
                MovementCoroutine = attacksManager.LaserAttack();
                break;
            case BossState.spawnTurrets:
                MovementCoroutine = SpawnTurretsState();
                break;
            case BossState.spawnMines:
                MovementCoroutine = SpawnMinesState();
                break;
            case BossState.spawnMobs:
                MovementCoroutine = SpawnMobsState();
                break;
            case BossState.orbWalking:
                MovementCoroutine = OrbWalkState();
                break;
            case BossState.dead:
                MovementCoroutine = DeadState();
                break;
            case BossState.setArmor:
                MovementCoroutine = ArmorState(currentMovementPhase.startingArmorType);
                break;
            default:
                break;
        }
        if (MovementCoroutine != null)
        {
            StartCoroutine(MovementCoroutine);
        }

        ShowRayOnCheckHidingSpot();
    }

    private void Update()
    {
        //Ani();
        AniSpeed();
        HealthBar();
        /*
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverSampleDistance, hidingSpotLayer);
        foreach (Collider i in hitColliders)
        {
            IsItAValidHidingPoint(widthOfTheBoss, i.transform.position);
        }
        */
        
    }

    public void ShowRayOnCheckHidingSpot()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverSampleDistance, hidingSpotLayer);
        foreach (Collider i in hitColliders)
        {
            IsItAValidHidingPoint(widthOfTheBoss, i.transform.position);
        }
    }

    //Animation speed for walking and running
    private void AniSpeed()
    {
        if (animator.GetInteger(aniDecision) == runningAni)
        {
            animator.speed = runAniSpeed;
        }
        else if (animator.GetInteger(aniDecision) == walkAni)
        {
            animator.speed = orbWalkAniSpeed;
        }
        else
        {
            animator.speed = 1;
        }
    }

    private void HealthBar()
    {
        Vector3 temp = new Vector3(PlayerController.puppet.cameraObj.transform.position.x, healthBarCanvasObject.transform.position.y, PlayerController.puppet.cameraObj.transform.position.z);
        healthBarCanvasObject.transform.LookAt(temp);
        healthBar.value = health.stat;
    }

    public void IdleAni()
    {
        animator.SetInteger(aniDecision, idleAni);
    }
    public void RunningAni()
    {
        animator.SetInteger(aniDecision, runningAni);
    }
    /*
    private void Ani()
    {
        if (animator == null) return;
        if (navMeshAgent.speed > 0)
        {
            animator.SetInteger(aniDecision, runAni);
            //animator.SetInteger
            //animator.SetFloat
            //animator.SetBool("isRunning", true);
        }
        else
        {
            IdleAni();
            //animator.SetBool("isRunning", false);
        }
    }
    */


    public IEnumerator TauntState()
    {
        navMeshAgent.speed = 0;
        //animator.SetBool("isTaunting", true);
        animator.SetInteger(aniDecision, tauntAni);
        //bossSpawner.SpawningThings();

        while (animator.GetInteger(aniDecision) == tauntAni)
        {
            yield return null;
        }
        
        //animator.SetBool("isTaunting", false);
        ExitTauntState();
        yield return null;
        //int r = Random.Range(0, 2);
    }

    //Change the state of Taunt
    public virtual void ExitTauntState()
    {
        Debug.Log("Boom");
        bossState = currentMovementPhase.tauntAttackDecision.GetTheNextRandomDicision();
    }


    public void EndTauntStateAni()
    {
        IdleAni();
    }

    public IEnumerator ArmorState(DamageType elementArmor)
    {
        SetArmor(elementArmor);
        ExitArmorState();
        yield return null;

    }

    public void SetArmor(DamageType elementArmor)
    {
        switch (elementArmor)
        {
            case DamageType.ice:
                EnterIceArmorState();
                break;

            case DamageType.fire:
                EnterFireArmorState();
                break;

            case DamageType.nuetral:
                EnterNoArmorState();
                break;
        }
    }
    public virtual void ExitArmorState()
    {
        //BossStageInteraction();
        //BossStageHandler();
        bossState = currentMovementPhase.getArmor.GetTheNextRandomDicision();
    }

    private void ChangeRandomElementState()
    {
        //int temp = Random.Range(0, AddAllDicision());
        if (Random.Range(0 , 2) == 0)
        {
            EnterFireArmorState();
        }
        else
        {
            EnterIceArmorState();
        }
    }

    private void ChangeElementState()
    {
        Debug.Log("Change Elemental");
        if (currentArmorElementType == DamageType.ice)
        {
            EnterFireArmorState();
        }
        else if (currentArmorElementType == DamageType.fire)
        {
            EnterIceArmorState();
        }
    }
    private void EnterIceArmorState()
    {
        WearIceArmor();
        currentArmorHealth.stat = currentArmorHealth.maximum;
    }

    private void WearIceArmor()
    {
        currentArmorElementType = DamageType.ice;
        iceArmorObject.SetActive(true);
        fireArmorObject.SetActive(false);
        SpawnVFX(iceArmorFormedVFX);
        //These interaction are for the boss health not the armor health
        ChangeDamageInteraction(DamageType.ice, DamageInteraction.immune);
        ChangeDamageInteraction(DamageType.fire, DamageInteraction.nuetral);
    }

    private void EnterFireArmorState()
    {
        WearFireArmor();
        currentArmorHealth.stat = currentArmorHealth.maximum;
    }

    private void WearFireArmor()
    {
        currentArmorElementType = DamageType.fire;
        fireArmorObject.SetActive(true);
        iceArmorObject.SetActive(false);
        SpawnVFX(fireArmorFormedVFX);
        //These interaction are for the boss health not the armor health
        ChangeDamageInteraction(DamageType.ice, DamageInteraction.nuetral);
        ChangeDamageInteraction(DamageType.fire, DamageInteraction.immune);
    }

    private void EnterNoArmorState()
    {
        WearNoArmor();
        currentArmorHealth.stat = currentArmorHealth.minimum;
    }

    private void WearNoArmor()
    {
        currentArmorElementType = DamageType.nuetral;
        fireArmorObject.SetActive(false);
        iceArmorObject.SetActive(false);
        //These interaction are for the boss health not the armor health
        ChangeDamageInteraction(DamageType.ice, DamageInteraction.nuetral);
        ChangeDamageInteraction(DamageType.fire, DamageInteraction.nuetral);
    }

    private IEnumerator TakeCoverState(Transform target)
    {
        navMeshAgent.speed = speed;
        RunningAni();
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
                nextCol = FindClosestValidHidingSpot(target, hitColliders);
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

    public Collider FindClosestValidHidingSpot(Transform target, Collider[] colliders)
    {
        Collider tempCol = null;
        List<Collider> colList = new List<Collider>();
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

    public Collider FindRandomValidHidingSpot(Transform target, Collider[] colliders)
    {
        //Collider tempCol = null;
        List<Collider> colList = new List<Collider>();
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
            if (prevousCoverPoint == thisCol.transform.position)
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

            colList.Add(thisCol);

        }

        //colList[Random.Range(0, colList.Count)];
        Debug.Log(colList.Count);
        Collider outputCollider;
        if (colList.Count > 0)
        {
            int randomIndex = Random.Range(0, colList.Count);
            outputCollider = colList[randomIndex];
        }
        else
        {
            outputCollider = FindClosestValidHidingSpot(target, colliders);
        }
        
        return outputCollider;
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
        IdleAni();
        // Check to see if this call is actually capable of ending
        if (secondsToWait == 0)
        {
            ExitInCoverState();
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
                //If the player detected the boss
                if (hit.collider.CompareTag("Player"))
                {
                    ExitInCoverState();
                    //bossState = BossState.takingCover;
                    yield break;
                }
            }

            yield return wait;
            //bossState = BossState.takingCover;
            
            ExitInCoverState();
            //yield break;
        }
    }
    public virtual void ExitInCoverState()
    {
        bossState = currentMovementPhase.coverDecision.GetTheNextRandomDicision();
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
    /*
    //This output a bossstate by calculate the bossstate using the decision and decision modifier during CoverAction decision.
    public BossState CoverActionDicision()
    {
        MovementDecision temp = new MovementDecision(coverActionDecision);

        // Adding all the decision modifers into the decision
        if (!IsPlayerWithinDistance(25))
        {
            temp.AddDicision(coverActionDecisionMod[0]);
        }
        if (health.stat / health.maximum <= 0.5)
        {
            temp.AddDicision(coverActionDecisionMod[1]);
        }
        // Find which bossstate to output
        return temp.GiveTheNextRandomDicision();
    }


    //This output a bossstate by calculate the bossstate using the decision and decision modifier during ambushed decision.
    public BossState AmbushedDicision()
    {
        MovementDecision temp = new MovementDecision(ambushedDecision);

        // Adding all the decision modifers into the decision
        if (health.stat / health.maximum > 0.5)
        {
            temp.AddDicision(ambushedDecisionMod[0]);
        }
        if (health.stat / health.maximum > 0.3)
        {
            temp.AddDicision(ambushedDecisionMod[1]);
        }
        if (IsPlayerWithinDistance(5))
        {
            temp.AddDicision(ambushedDecisionMod[2]);
        }
        if (!IsPlayerWithinDistance(25))
        {
            temp.AddDicision(ambushedDecisionMod[3]);
        }
        // Find which bossstate to output
        return temp.GiveTheNextRandomDicision();
    }

    //This output a bossstate by calculate the bossstate using the decision and decision modifier during attack decision.
    public BossState AttackDicision()
    {
        MovementDecision temp = new MovementDecision(attackDecision);

        // Adding all the decision modifers into the decision
        if (!IsPlayerWithinDistance(50))
        {
            temp.AddDicision(attackDecisionMod[0]);
        }
        if (!IsPlayerWithinDistance(25))
        {
            temp.AddDicision(attackDecisionMod[1]);
        }
        if (IsPlayerWithinDistance(25))
        {
            temp.AddDicision(attackDecisionMod[2]);
        }
        if (IsPlayerWithinDistance(10))
        {
            temp.AddDicision(attackDecisionMod[3]);
        }
        temp.DisplayLog();
        // Find which bossstate to output
        return temp.GiveTheNextRandomDicision();
    }
    */

    public IEnumerator TeleportingToCoverState(Transform target)
    {
        //WaitForSeconds wait = new WaitForSeconds(coverUpdateFrequency);
        IdleAni();
        //Debug.Log("Test");
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, teleportSampleDistance, hidingSpotLayer);

            if (hitColliders.Length == 0)
            {
                Debug.Log("Unable to find point to teleport");
            }
            else
            {
                //Find the right collider that's behind the player
                Collider tempCol = FindRandomValidHidingSpot(target, hitColliders);

                // If null, then boss is unable to find a spot to teleport behind
                if (tempCol == null)
                {
                    Debug.Log("No valid teleport spot");
                    bossState = BossState.takingCover;

                }
                else
                {
                    //Teleport boss
                    //Spawn VFX when teleport
                    SpawnVFX(teleportVFX);
                    this.transform.position = tempCol.transform.position;
                    //SpawnTeleportVPX();
                    ExitTeleportingState();
                }
            }

            yield return null;
        }
    }
    
    public virtual void ExitTeleportingState()
    {
        bossState = currentMovementPhase.teleportDecision.GetTheNextRandomDicision();
    }

    public IEnumerator TeleportingBehindState(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(coverUpdateFrequency);
        IdleAni();
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
                //Find the right collider that's behind the player
                Collider tempCol = FindValidBehindPlayerSpot(target, hitColliders);

                // If null, then boss is unable to find a spot to teleport behind
                if (tempCol == null)
                {
                    // If there's no available spot that is behind the player, then seach all the spot
                    tempCol = FindClosestValidHidingSpot(target, hitColliders);

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

    public Collider FindValidBehindPlayerSpot(Transform target, Collider[] colliders)
    {
        Collider tempCol = null;
        foreach (Collider thisCol in colliders)
        {
            Vector3 vectorToColloder = thisCol.transform.position - target.position;

            //Check if the the spot is behind the player
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

    public IEnumerator OrbWalkState()
    {
        EnteringOrbWalk();
        yield return null;
        animator.SetInteger(aniDecision, walkAni);
        yield return null;
        while (true)
        {
            MovementSystem();
            yield return null;
        }
        //yield return null;
    }

    public void EnteringOrbWalk()
    {
        //Debug.Log("Reset");
        navMeshAgent.SetDestination(destination);
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.angularSpeed = 0;
        navMeshAgent.speed = orbWalkSpeed;
        navMeshAgent.acceleration = orbWalkAcceleration;

        orbWalkingTime = Random.Range(minEndOrbWalkingTime, maxEndOrbWalkingTime);
        endOrbWalkingTimer = orbWalkingTime;
        //dodgeTime = orbWalkingTime - Random.Range(minDodgeTime, maxDodgeTime);
        IdleAni();
        
        if (Random.Range(0, 2) == 0)
        {
            ChangeDirection();
        }
    }
    

    // Update is called once per frame
    public void MovementSystem()
    {
        if (endOrbWalkingTimer <= 0)
        {
            endOrbWalkingTimer = 0;
        }
        else
        {
            endOrbWalkingTimer -= Time.fixedDeltaTime;
        }

        //Vector3 veiwToPlayerMesh = transform.position - viewPoint.transform.position;
        //transform.forward = Vector3.RotateTowards(transform.forward, veiwToPlayerMesh, turnSpeed * Time.deltaTime, 0.0f);

        Vector3 lookDirection;
        lookDirection = (PlayerController.puppet.transform.position - transform.position).normalized;


        lookDirection.y = 0;
        rotateGoalWithOutY = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateGoalWithOutY, turnSpeed * Time.fixedDeltaTime);

        //xlookDirection.x = 0;
        //xlookDirection.y = 0;

        //p1.z -= 1;

        NavMeshHit hit;

        

        if (!IsPlayerWithinDistance(farthestStoppingDistance))
        {
            moveVector = moveForwardVector;
        }
        else if (IsPlayerWithinDistance(closestStoppingDistance))
        {
            moveVector = moveBackwardVector;
        }
        else
        {
            moveVector = moveMidRangeVector;
        }

        animator.SetFloat(aniLeftRightDecision, moveVector.normalized.x);
        animator.SetFloat(aniForwardBackDecision, moveVector.normalized.z);

        Vector3 tempMoveVector = moveVector.normalized * navMeshDetactionDistance;
        if (NavMesh.SamplePosition(transform.position + (transform.right.normalized * tempMoveVector.x) + (transform.forward.normalized * tempMoveVector.z), out hit, navMeshDetactionRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
        else
        {
            Debug.Log("Hey! YoY");
            ChangeDirection();
            //changeDirectionTimer = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
            // moveSideBackwardVector.x *= -1;
        }

        if (endOrbWalkingTimer <= 0)
        {
            //ChangeDirection();
            IdleAni();
            navMeshAgent.stoppingDistance = 0;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.speed = speed;
            navMeshAgent.acceleration = acceleration;
            ExitOrbWalkState();
            //changeDirectionTimer = dodgeDecision.GetRandomIndex() == 0 ? Random.Range(minChangeDirectionTime, maxChangeDirectionTime) : dodgeTime;
        }
    }

    public virtual void ExitOrbWalkState()
    {
        //BossStageInteraction();
        //BossStageHandler();
        bossState = currentMovementPhase.orbwalkDecision.GetTheNextRandomDicision();
        //bossState = 
    }
    private void ChangeDirection()
    {
        moveForwardVector.x *= -1;
        moveMidRangeVector.x *= -1;
        moveBackwardVector.x *= -1;
    }
    public IEnumerator SpawnMinesState()
    {
        mobSpawnerController.SpawningBaseOnIndex(1);
        /*
        navMeshAgent.speed = 0;
        animator.SetInteger(aniDecision, tauntAni);

        while (animator.GetInteger(aniDecision) == tauntAni)
        {
            yield return null;
        }
        */
        ExitSpawnMinesState();
        yield return null;
        //mobSpawnerController.SpawningBaseOnIndex(1);
        //yield return null;
        //ExitSpawnMinesState();

    }
    public virtual void ExitSpawnMinesState()
    {
        //bossState = BossState.takingCover;
        //BossStageInteraction();
        //BossStageHandler();
        bossState = currentMovementPhase.dropMineDecision.GetTheNextRandomDicision();
    }
    public IEnumerator SpawnTurretsState()
    {
        mobSpawnerController.SpawningBaseOnIndex(0);
        /*
        navMeshAgent.speed = 0;
        animator.SetInteger(aniDecision, tauntAni);

        while (animator.GetInteger(aniDecision) == tauntAni)
        {
            yield return null;
        }
        */
        ExitSpawnTurretState();
        yield return null;
        //mobSpawnerController.SpawningBaseOnIndex(0);
        //yield return null;
        //ExitSpawnTurretState();

    }
    public virtual void ExitSpawnTurretState()
    {
        //bossState = BossState.takingCover;
        //BossStageInteraction();
        //BossStageHandler();
        bossState = currentMovementPhase.dropTurretDecision.GetTheNextRandomDicision();
    }

    public IEnumerator SpawnMobsState()
    {
        mobSpawnerController.SpawningBaseOnIndex(2);
        /*
        navMeshAgent.speed = 0;
        animator.SetInteger(aniDecision, tauntAni);

        while (animator.GetInteger(aniDecision) == tauntAni)
        {
            yield return null;
        }
        */
        ExitSpawnMobsState();
        yield return null;
        //mobSpawnerController.SpawningBaseOnIndex(0);
        //yield return null;
        //ExitSpawnTurretState();

    }
    public virtual void ExitSpawnMobsState()
    {
        //bossState = BossState.takingCover;
        //BossStageInteraction();
        //BossStageHandler();
        bossState = currentMovementPhase.spawnMobDecision.GetTheNextRandomDicision();
    }

    public void TauntSpawnMobAni()
    {
        Debug.Log("SpawnStuff");
        if (bossState == BossState.spawnMines)
        {
            mobSpawnerController.SpawningBaseOnIndex(1);
        } 
        else if (bossState == BossState.spawnTurrets)
        {
            mobSpawnerController.SpawningBaseOnIndex(0);
        }
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


    public override void Damage(float damageAmount, Vector3 hitPosition, DamageType damageType = DamageType.nuetral)
    {
        float damage;
        if (currentArmorElementType != DamageType.nuetral)
        {
            if (damageImmunities.Contains(damageType))
            {
                if (usesDamageText)
                {
                    GameObject damageText = GetDamageText(damageType);
                    damageText.GetComponent<DamageText>().UpdateDamage(hitPosition, 0, damageType);
                }

                return;
            }

            damage = DamageCalculation(damageAmount, damageType);

            if (usesDamageText)
            {
                GameObject damageText = GetDamageText(damageType);
                damageText.GetComponent<DamageText>().UpdateDamage(hitPosition, damage, damageType);
            }

            currentArmorHealth.AddToStat(-damage);

            //Check for the health of armor
            if (currentArmorHealth.stat <= currentArmorHealth.minimum)
            {
                if (currentArmorElementType == DamageType.ice)
                {
                    SpawnVFX(iceArmorDestroyedVFX);
                }
                if (currentArmorElementType == DamageType.fire)
                {
                    SpawnVFX(fireArmorDestroyedVFX);
                }
                currentArmorHealth.stat = currentArmorHealth.minimum;
                //EneterNextPhase();
                EnterNoArmorState();
                health.AddToStat(-health.maximum * currenthPReductionPercentOnArmorBreak/100);
            }
            StartCoroutine(InvincibilityFrames());
            return;
        }

        Debug.Log("Health decrease");
        base.Damage(damageAmount, hitPosition, damageType);
        /*
        if (damageImmunities.Contains(damageType))
        {
            if (usesDamageText)
            {
                GameObject damageText = GetDamageText(damageType);
                damageText.GetComponent<DamageText>().UpdateDamage(hitPosition, 0, damageType);
            }

            return;
        }

        damage = DamageCalculation(damageAmount, damageType);


        if (usesDamageText)
        {
            GameObject damageText = GetDamageText(damageType);
            damageText.GetComponent<DamageText>().UpdateDamage(hitPosition, damage, damageType);
        }
        if (!inInvincibilityFrames)
        {
            health.AddToStat(-damage);
            //StartCoroutine(CheckArmorPhase());
            
        }

        if (health.stat <= health.minimum && !dead)
        {
            CommitDie();
        }
        */
    }
    /*
    public IEnumerator CheckArmorPhase()
    {
        BossPhase tempPhase = bossPhase;
        yield return new WaitForSeconds(0.5f);
        if (tempPhase != bossPhase)
        {
            bossState = BossState.setArmor;
        }
    }
    */
    public IEnumerator DeadState()
    {
        navMeshAgent.speed = 0;
        animator.SetBool(aniDeathDecision, true);
        yield return null;
        //animator.SetBool(aniDeathDecision, false);
        //isDeadAni = false;
        yield return new WaitForSeconds(activationTimeAfterDeath);
        animator.SetBool(aniDeathDecision, false);
        Dead();

    }

    public override void CommitDie()
    {
        
        base.CommitDie();
        if (isAbleToPlayDeathAni)
        {
            bossState = BossState.dead;
        }
        else
        {
            Dead();
        }
        
    }

    public virtual void Dead()
    {
        GeneralManager.instance.WinGame();
    }

    
}
