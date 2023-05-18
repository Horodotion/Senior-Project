using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[System.Serializable]
public class OrbWalkController : BossEnemyController
{
    //public GameObject viewPoint;
    //public GameObject sp;
    //public GameObject projectile;
    //private bool isAbleFire;
    //public float coolDown;

    //private NavMeshAgent navMeshAgent;
    [SerializeField] private GameObject meshObject;

    private Vector3 destination;
    public float acceleration;
    public float turnSpeed;
    [SerializeField] private bool isYRotaionNeeded;
    private Quaternion rotateGoal;
    private Quaternion rotateGoalWithOutY;

    [Range(0.1f, 10)] public float navMeshDetactionRadius;
    [Range(0.1f, 10)] public float navMeshDetactionDistance;

    public float lostPlayerRange;
    public float closestStoppingDistance;
    public float farthestStoppingDistance;
    //public float maxChangeDirectionTime;
    public float minChangeDirectionTime;
    public float maxChangeDirectionTime;
    public float dodgeTime;

    //[SerializeField] WeightedDecision dodgeDecision;// = new WeightedDecision();

    private float changeDirectionTimer;
    public Vector3 moveForwardVector;
    public Vector3 moveMidRangeVector;
    public Vector3 moveBackwardVector;
    public Vector3 moveVector;

    public IEnumerator OrbWalkState()
    {
        EnteringMovement();
        while (true)
        {
            MovementSystem();
        }
        yield return null;
    }

    public void EnteringMovement()
    {
        navMeshAgent.SetDestination(destination);
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.angularSpeed = 0;
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = acceleration;
        changeDirectionTimer = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
        if (Random.Range(0, 2) == 0)
        {
            ChangeDirection();
        }
    }

    // Update is called once per frame
    public void MovementSystem()
    {
        if (changeDirectionTimer <= 0)
        {
            changeDirectionTimer = 0;
        }
        else
        {
            changeDirectionTimer -= Time.deltaTime;
        }

        //Vector3 veiwToPlayerMesh = transform.position - viewPoint.transform.position;
        //transform.forward = Vector3.RotateTowards(transform.forward, veiwToPlayerMesh, turnSpeed * Time.deltaTime, 0.0f);

        Vector3 lookDirection;
        lookDirection = (PlayerController.puppet.transform.position - transform.position).normalized;

        float step = turnSpeed * Time.deltaTime;
        if (isYRotaionNeeded)
        {
            rotateGoal = Quaternion.LookRotation(lookDirection);
            meshObject.transform.rotation = Quaternion.RotateTowards(meshObject.transform.rotation, rotateGoal, step);
        }
        

        lookDirection.y = 0;
        rotateGoalWithOutY = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateGoalWithOutY, step);

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

        Vector3 tempMoveVector = moveVector.normalized * navMeshDetactionDistance;
        if (NavMesh.SamplePosition(transform.position + (transform.right.normalized * tempMoveVector.x) + (transform.forward.normalized * tempMoveVector.z), out hit, navMeshDetactionRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
        else
        {
            Debug.Log("Hey! YoY");
            ChangeDirection();
            changeDirectionTimer = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
            // moveSideBackwardVector.x *= -1;
        }

        if (changeDirectionTimer <= 0)
        {
            ChangeDirection();
            //changeDirectionTimer = dodgeDecision.GetRandomIndex() == 0 ? Random.Range(minChangeDirectionTime, maxChangeDirectionTime) : dodgeTime;
        }
    }

    private void ChangeDirection()
    {
        moveForwardVector.x *= -1;
        moveMidRangeVector.x *= -1;
        moveBackwardVector.x *= -1;
    }
}
