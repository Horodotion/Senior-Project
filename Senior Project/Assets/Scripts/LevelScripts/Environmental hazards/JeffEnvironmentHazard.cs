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
    public GameObject telePoof;
    public Transform poofSpawnPoint;
    public IceDisintegration iceWallToMelt;
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
    public bool connectedIceWall = false;

    [Header("Disintegration")]
    public GameObject iceMesh;
    public GameObject boneBesh;
    public GameObject bodyMesh;
    private SkinnedMeshRenderer iceRend, boneRend, bodyRend;
    private float shaderValue;
    public float deltaRate = 2;
    public float deltaValue = .1f;
    private bool disintegrating = false;

    public AudioSource ourAudioSource;
    public AudioClip meleeBugle;
    public AudioClip projectileBugle;
    public AudioClip laserBugle;
    public AudioClip tauntBugle;
    public AudioClip teleportBugle;

    private void Awake()
    {
        if (nextJeff != null)
        {
            nextJeff.SetActive(false);
        }
        player = GameObject.FindObjectOfType<PlayerPuppet>().transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ourAudioSource = GetComponent<AudioSource>();

        iceRend = iceMesh.GetComponent<SkinnedMeshRenderer>();
        boneRend = boneBesh.GetComponent<SkinnedMeshRenderer>();
        bodyRend = bodyMesh.GetComponent<SkinnedMeshRenderer>();
        shaderValue = bodyRend.material.GetFloat("_Disintigration");
        Instantiate(telePoof, new Vector3(poofSpawnPoint.position.x, poofSpawnPoint.position.y, poofSpawnPoint.position.z), Quaternion.identity);
    }

    void Start()
    {
        GeneralManager.AddEventToDict(eventFlag);
        if (GeneralManager.HasEventBeenTriggered(eventFlag))
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
        GeneralManager.SetEventFlag(eventFlag);
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
            // Debug.Log("I attacked!");
            

            if (PlayerController.instance.temperature.stat >= 0)
            {
                GameObject thisProjectile1 = SpawnManager.instance.GetGameObject(fireProjectile, SpawnType.projectile);
                //Debug.Log(thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController testController));
                if (thisProjectile1.TryGetComponent<ProjectileController>(out ProjectileController projectileController))
                {
                    projectileController.transform.position = shootPoint.transform.position;
                    Vector3 direction = (PlayerController.puppet.transform.position - shootPoint.transform.position).normalized;

                    Quaternion directionToMove = Quaternion.LookRotation(direction, transform.up);
                    projectileController.transform.rotation = directionToMove;
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
                    Vector3 direction = (PlayerController.puppet.transform.position - shootPoint.transform.position).normalized;

                    Quaternion directionToMove = Quaternion.LookRotation(direction, transform.up);
                    projectileController.transform.rotation = directionToMove;
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
        ourAudioSource.clip = projectileBugle;
        ourAudioSource.Play();
    }

    private void TauntSpawnMobAni()
    {
        anim.SetInteger("idleDecision", 3);
        ourAudioSource.clip = laserBugle;
        ourAudioSource.Play();
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
        if (connectedIceWall && iceWallToMelt != null)
        {
            iceWallToMelt.disintegrate = true;
        }
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
        Instantiate(telePoof, new Vector3(poofSpawnPoint.position.x, poofSpawnPoint.position.y, poofSpawnPoint.position.z), Quaternion.identity);
        Invoke(nameof(Disappear), despawnTime);
    }

    private void Disappear()
    {
        if(cascadingJeff)
        {
            nextJeff.SetActive(true);
            Instantiate(telePoof, new Vector3(poofSpawnPoint.position.x, poofSpawnPoint.position.y, poofSpawnPoint.position.z), Quaternion.identity);
            Destroy(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
