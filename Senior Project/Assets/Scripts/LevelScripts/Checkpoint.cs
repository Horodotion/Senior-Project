using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : TriggerScript
{
    public EventFlag eventFlag;

    public static Vector3 playerSpawn;
    public static bool originalSpawnLocated;
    public static Vector3 playerLookDirection;
    public static Checkpoint currentCheckpoint;

    void Start()
    {
        if (!originalSpawnLocated && PlayerController.puppet != null)
        {
            originalSpawnLocated = true;
            playerSpawn = PlayerController.puppet.transform.position;
            playerLookDirection = PlayerController.puppet.transform.localEulerAngles;
        }

        GeneralManager.instance.AddEventToDict(eventFlag);
        if (GeneralManager.HasEventBeenTriggered(eventFlag))
        {
            gameObject.SetActive(false);
        }
    }

    public override void ActionToTrigger()
    {        
        if (!GeneralManager.HasEventBeenTriggered(eventFlag))
        {
            currentCheckpoint = this;
            GeneralManager.instance.SetEventFlag(eventFlag);

            // playerSpawn = PlayerController.puppet.transform.position;
            // playerLookDirection = new Vector3(transform.forward.x, transform.forward.y, 0);
        }
        // else
        // {
        //     playerSpawn = PlayerController.puppet.transform.position;
        //     playerLookDirection = new Vector3(transform.forward.x, transform.forward.y, 0);
        // }
    }

    public static Vector3 GetPlayerRespawnPosition()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.transform.position;
        }
        
        return playerSpawn;
    }

    public static Vector3 GetPlayerRespawnRotation()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.transform.eulerAngles;
        }
        
        return playerLookDirection;
    }
}