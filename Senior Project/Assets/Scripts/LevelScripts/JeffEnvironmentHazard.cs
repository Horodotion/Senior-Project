using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public bool rangedAttack;
    public bool dropTurret;
    public bool fireElem;


    [Header("State Checks")]
    public float attackRange;
    public bool playerInAttackRange;
    public bool playerFound;
    public bool hasAttacked;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerPuppet>().transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        if (playerInAttackRange && !playerFound)
        {
            if (rangedAttack && !playerFound)
            {
                Debug.Log("Found You!");
                playerFound = true;
                Attack();
            }
            else if(dropTurret && !playerFound)
            {
                Debug.Log("Dropping Turret");
                playerFound = true;
                SpawnTurret();
            }
            else if (!rangedAttack && !dropTurret && !playerFound)
            {
                Debug.Log("Running");
                playerFound = true;
                Invoke(nameof(RunAway), 1f);
            }
        }
    }


    private void Attack()
    {
        //Stop Enemy and look at player
        transform.LookAt(player);

        if (!hasAttacked)
        {
            Debug.Log("I attacked!");

            if(fireElem)
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
        Invoke(nameof(RunAway), 2f);
    }

    private void RunAway()
    {
        //transform.LookAt(runawayPoint);
        anim.SetBool("isRunning", true);
        agent.SetDestination(runawayPoint.position);
        Invoke(nameof(Disappear), 2f);
    }


    private void Disappear()
    {
        Destroy(gameObject, 1f);
    }
}
