using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGate : TriggerScript
{
    public GameObject wallMesh;
    public Transform destination;
    public float speed;
    public bool playerIn;

    private void FixedUpdate()
    {
        if(playerIn)
        {
            Invoke(nameof(Descend), 1f);
        }
    }

    // Update is called once per frame
    public override void ActionToTrigger()
    {
        playerIn = true;
    }

    public void Descend()
    {
        wallMesh.transform.position = Vector3.MoveTowards(wallMesh.transform.position, destination.position, speed * Time.deltaTime);
    }

}
