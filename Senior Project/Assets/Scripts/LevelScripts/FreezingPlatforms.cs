using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingPlatforms : EnemyController
{
    [Header("References")]
    private float recoveryTimer;
    public float recoveryRate;
    public float flashingPercentage = 0.6f;
    public float startDelay; 
    [SerializeField] private Transform targetA, targetB;
    public Material liveMaterial;
    public Material frozenMaterial;
    public GameObject colliderBox;
    public GameObject unfreezeVFX;
    public Transform vfxSpawnPoint;
    public bool spinning = false;
    public bool isDead = false;
    
    private int flashing = 1;
    private float startTimer = 0f;
    public float speed = 0.1f; //Change this to suit your game.
    private bool switching = false;

    [Header("SFX")]
    public AudioClip freezingSFX;
    public AudioClip thawingSFX;

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
        /*
       if(recoveryTimer >= (recoveryRate * flashingPercentage) && dead)
        {
            if(flashing == 1)
            {
                GetComponent<MeshRenderer>().material = frozenMaterial;
                Invoke(nameof(Flashing), 0.5f);
            }
            else
            {
                GetComponent<MeshRenderer>().material = liveMaterial;
                Invoke(nameof(Flashing), 0.5f);
            }
        }
        */
        if(dead)
        {
            if (recoveryTimer >= (recoveryRate * flashingPercentage))
            {
                GetComponent<MeshRenderer>().material = frozenMaterial;
                FlashingLive();
            }

            if (recoveryTimer < recoveryRate)
            {
                recoveryTimer += Time.deltaTime;
            }
            else
            {
                Recover();
            }
            return;
        }

        if(!spinning)
        {
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

    }

    public override void CommitDie()
    {
        base.CommitDie();
        isDead = true;
        GetComponent<MeshRenderer>().material = frozenMaterial;
        AudioSource.PlayClipAtPoint(freezingSFX, transform.position);

        if (colliderBox != null)
        {
            colliderBox.SetActive(true);
        }
        recoveryTimer = 0;
    }

    public void Recover()
    {
        health.ResetStat();
        isDead = false;
        GetComponent<MeshRenderer>().material = liveMaterial;
        if (colliderBox != null)
        {
            colliderBox.SetActive(false);
        }
        Instantiate(unfreezeVFX, new Vector3(vfxSpawnPoint.position.x, vfxSpawnPoint.position.y, vfxSpawnPoint.position.z), Quaternion.identity);
        AudioSource.PlayClipAtPoint(thawingSFX, transform.position);
        dead = false;
    }

    public void FlashingLive()
    {
        if (dead)
        {
            GetComponent<MeshRenderer>().material = frozenMaterial;
            Invoke(nameof(FlashingDead), 0.2f);
        }
    }

    public void FlashingDead()
    {
        if (dead)
        {
            GetComponent<MeshRenderer>().material = liveMaterial;
            Invoke(nameof(FlashingLive), 0.2f);
        }
    }

}
