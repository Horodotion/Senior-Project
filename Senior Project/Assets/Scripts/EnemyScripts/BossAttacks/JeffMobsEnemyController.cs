using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JeffMobsEnemyController : BossEnemyController
{

    public override void Awake()
    {

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;
        navMeshAgent.acceleration = acceleration;
        attacksManager = GetComponent<AttacksManager>();
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
    // Start is called before the first frame update
    public override void Start()
    {
        ResetHealthBar();

        bossState = BossState.idle;

        bossState = BossState.meleeAttack;

        //navMeshAgent.SetDestination(PlayerController.puppet.transform.position);
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        
    }
    public override void EneterNextPhase()
    {
        
    }
    public override void Dead()
    {
        Destroy(this);
    }
}
