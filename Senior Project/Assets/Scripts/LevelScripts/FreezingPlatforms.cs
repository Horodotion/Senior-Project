using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingPlatforms : EnemyController
{
    [Header("References")]
    private float recoveryTimer;
    public float recoveryRate;
    public float startDelay; 
    [SerializeField] private Transform targetA, targetB;
    public Material liveMaterial;
    public Material frozenMaterial;
    public GameObject colliderBox;


    private float startTimer = 0f;
    public float speed = 0.1f; //Change this to suit your game.
    private bool switching = false;

    private void Start()
    {
        if(colliderBox != null)
        {
            colliderBox.SetActive(false);
        }
    }

    void FixedUpdate()
    {
       if(startTimer < startDelay)
        {
            startTimer += Time.deltaTime;
            return;
        }
        
        if(dead)
        {
            if(recoveryTimer < recoveryRate)
            {
                recoveryTimer += Time.deltaTime;
            }
            else
            {
                Recover();
            }
            return;
        }

        if (!switching)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetB.position, speed * Time.deltaTime);
        }
        else if (switching)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetA.position, speed * Time.deltaTime);
        }
        if (transform.position == targetB.position)
        {
            switching = true;
        }
        else if (transform.position == targetA.position)
        {
            switching = false;
        }

    }

    public override void CommitDie()
    {
        base.CommitDie();
        GetComponent<MeshRenderer>().material = frozenMaterial;
        
        if (colliderBox != null)
        {
            colliderBox.SetActive(true);
        }
        recoveryTimer = 0;
    }

    public void Recover()
    {
        health.ResetStat();
        GetComponent<MeshRenderer>().material = liveMaterial;
        if (colliderBox != null)
        {
            colliderBox.SetActive(false);
        }
        dead = false;
    }

}
