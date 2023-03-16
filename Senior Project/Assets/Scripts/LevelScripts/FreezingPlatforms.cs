using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingPlatforms : EnemyController
{

    //Adjust this to change speed
    [SerializeField] float speed = 5f;
    //Adjust this to change how high it goes
    [SerializeField] float height = 0.5f;
    private float recoveryTimer;
    public float recoveryRate;
    //public float startDelay;
    //private float startTimer = 0f;
    

    Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }

    void FixedUpdate()
    {
       //if(startTimer < startDelay)
       // {
            //startTimer += Time.deltaTime;
            //return;
       // }
        
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
        //calculate what the new Y position will be
        float newY = Mathf.Sin(Time.time * speed) * height + pos.y;
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public override void CommitDie()
    {
        base.CommitDie();
        recoveryTimer = 0;
    }

    public void Recover()
    {
        health.ResetStat();
        dead = false;
    }

}
