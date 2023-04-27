using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HazardType
{
    projectile,
    turret,
    mine,
    other
}

public class JeffEnvironmentHazard : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public Transform runawayPoint;
    public GameObject fireProjectile;
    public GameObject iceProjectile;
    public GameObject myTurret;
    public Transform shootPoint;
    public Transform turretDropPoint;
    public LayerMask whatIsPlayer;
    // public bool rangedAttack;
    // public bool dropTurret;
    // public bool fireElem;


    [Header("State Checks")]
    public HazardType hazardType;
    public float attackRange;
    public float despawnTime;
    public bool playerFound;
    public bool hasAttacked;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerPuppet>().transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void FixedUpdate()
    {
        if (!playerFound && Vector3.Distance(transform.position, PlayerController.puppet.transform.position) <= attackRange)
        {
            PlayerHasBeenFound();
        }
    }

    public void PlayerHasBeenFound()
    {
        playerFound = true;

        switch(hazardType)
        {
            case HazardType.projectile:
                Attack();
                break;

            case HazardType.turret:
            case HazardType.mine:
                SpawnTurret();
                break;

            default:
                Invoke(nameof(RunAway), 1f);
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1);
    }

    private void Attack()
    {
        //Stop Enemy and look at player
        transform.LookAt(player);

        if (!hasAttacked)
        {
            Debug.Log("I attacked!");

            if(PlayerController.instance.temperature.stat >= 0)
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
            

            hasAttacked = true;
            Invoke(nameof(RunAway), 1f);
        }
    }

    private void SpawnTurret()
    {
        Instantiate(myTurret, turretDropPoint.transform.position, Quaternion.identity);
        Invoke(nameof(RunAway), 1f);
    }

    private void RunAway()
    {
        //transform.LookAt(runawayPoint);
        anim.SetBool("isRunning", true);
        agent.SetDestination(runawayPoint.position);
        Invoke(nameof(Disappear), despawnTime);
    }


    private void Disappear()
    {
        Destroy(gameObject);
    }
}
