using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum HazardType
{
    projectile,
    turret,
    mine,
    wall,
    other
}

public class JeffEnvironmentHazard : MonoBehaviour
{
    public EventFlag eventFlag;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public Transform runawayPoint;
    public GameObject fireProjectile;
    public GameObject iceProjectile;
    public GameObject myTurret;
    public GameObject wallHazard; 
    public Transform shootPoint;
    public Transform turretDropPoint;
    public LayerMask whatIsPlayer;
    public bool cascadingJeff;
    public GameObject nextJeff = null;
    public bool instantDisintegrate;
    // public bool dropTurret;
    // public bool fireElem;


    [Header("State Checks")]
    public HazardType hazardType;
    public float attackRange;
    public float criticalRange;
    public float despawnTime;
    public bool repeatingAttack;
    private bool playerFound;
    private bool hasAttacked;
    public bool extraDrop;
    private bool dropped = false;

    [Header("Disintegration")]
    public GameObject iceMesh;
    public GameObject boneBesh;
    public GameObject bodyMesh;
    private SkinnedMeshRenderer iceRend, boneRend, bodyRend;
    private float shaderValue;
    public float deltaRate = 2;
    public float deltaValue = .1f;
    private bool disintegrating = false;


    private void Awake()
    {
        if (nextJeff != null)
        {
            nextJeff.SetActive(false);
        }
        player = GameObject.FindObjectOfType<PlayerPuppet>().transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        iceRend = iceMesh.GetComponent<SkinnedMeshRenderer>();
        boneRend = boneBesh.GetComponent<SkinnedMeshRenderer>();
        bodyRend = bodyMesh.GetComponent<SkinnedMeshRenderer>();
        shaderValue = bodyRend.material.GetFloat("_Disintigration");
    }

    void Start()
    {
        GeneralManager.instance.AddEventToDict(eventFlag);
        if (GeneralManager.instance.eventFlags[eventFlag.eventID].eventTriggered)
        {
            gameObject.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        if (!playerFound && Vector3.Distance(transform.position, PlayerController.puppet.transform.position) <= attackRange)
        {
            PlayerHasBeenFound();
        }

        if (playerFound && repeatingAttack && Vector3.Distance(transform.position, PlayerController.puppet.transform.position) <= criticalRange)
        {
            RunAway();
        }

        if(disintegrating)
        {
            if(shaderValue < 1)
            {
                shaderValue += deltaValue * (deltaRate * Time.deltaTime);
                Debug.Log(shaderValue);
                iceRend.material.SetFloat("_Disintigration", shaderValue);
                bodyRend.material.SetFloat("_Disintigration", shaderValue);
                boneRend.material.SetFloat("_Disintigration", shaderValue);
            }
        }
    }

    public void PlayerHasBeenFound()
    {
        GeneralManager.instance.DisablePriorEvents(eventFlag.eventID);
        playerFound = true;

        switch(hazardType)
        {
            case HazardType.projectile:
                Attack();
                break;

            case HazardType.turret:
            case HazardType.mine:
                TauntSpawnMobAni();
                break;

            case HazardType.wall:
                ActivateWall();
                break;

            default:
                Invoke(nameof(RunAway), 1f);
                break;
        }
    }


    private void ActivateWall()
    {
        if (wallHazard != null)
        {
            wallHazard.SetActive(true);

            if (extraDrop)
            {
                Invoke(nameof(TauntSpawnMobAni), 1f);
            }
            else
            {
                Invoke(nameof(RunAway), 1f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1);
    }

    private void FireAProjectile()
    {
        if (!hasAttacked)
        {
            Debug.Log("I attacked!");

            if (PlayerController.instance.temperature.stat >= 0)
            {
                GameObject thisProjectile1 = SpawnManager.instance.GetGameObject(fireProjectile, SpawnType.projectile);
                //Debug.Log(thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController testController));
                if (thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController projectileController))
                {
                    projectileController.transform.position = shootPoint.transform.position;
                    projectileController.transform.rotation = shootPoint.transform.rotation;
                    projectileController.LaunchProjectile();
                }
            }
            else
            {
                GameObject thisProjectile1 = SpawnManager.instance.GetGameObject(iceProjectile, SpawnType.projectile);
                //Debug.Log(thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController testController));
                if (thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController projectileController))
                {
                    projectileController.transform.position = shootPoint.transform.position;
                    projectileController.transform.rotation = shootPoint.transform.rotation;
                    projectileController.LaunchProjectile();
                }
            }

            if (repeatingAttack)
            {
                Invoke(nameof(Attack), 1.5f);
            }
            else
            {
                anim.SetInteger("idleDecision", 1);
                hasAttacked = true;
                RunAway();
            }
        }
    }

    public void ExitAttackAnimation()
    {

    }

    private void Attack()
    {
        //Stop Enemy and look at player
        transform.LookAt(player);
        anim.SetInteger("idleDecision", 4);
    }

    private void TauntSpawnMobAni()
    {
        anim.SetInteger("idleDecision", 3);
        Invoke(nameof(EndTauntStateAni), 2.4f);
    }

    private void EndTauntStateAni()
    {
        if (!dropped)
        {
            Instantiate(myTurret, turretDropPoint.transform.position, Quaternion.identity);
            dropped = true;
        }

        RunAway();
    }

    private void RunAway()
    {
        //transform.LookAt(runawayPoint);
        //anim.SetBool("isRunning", true);
        anim.speed = 1;
        anim.SetInteger("idleDecision", 1);
        agent.SetDestination(runawayPoint.position);
        if(instantDisintegrate)
        {
            disintegrating = true;
        }
        Invoke(nameof(Disintegrate), despawnTime);
    }

    private void Disintegrate()
    {
        disintegrating = true;
        Invoke(nameof(Disappear), despawnTime);
    }

    private void Disappear()
    {
        if(cascadingJeff)
        {
            nextJeff.SetActive(true);
            Destroy(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
