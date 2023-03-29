using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : TriggerScript
{
    public static Vector3 playerSpawn;
    public static Checkpoint currentCheckpoint;
    public static Vector3 playerLookDirection;
    public static Vector3 playerCameraDirection;

    void Start()
    {
        if (playerSpawn == Vector3.zero && PlayerController.puppet != null)
        {
            playerSpawn = PlayerController.puppet.transform.position;
            playerLookDirection = PlayerController.puppet.transform.localEulerAngles;
            playerCameraDirection = PlayerController.puppet.cameraObj.transform.localEulerAngles;
        }
    }

    public override void ActionToTrigger()
    {
        currentCheckpoint = this;
        playerLookDirection = new Vector3(transform.forward.x, transform.forward.y, 0);
    }
}