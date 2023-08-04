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
        if (GeneralManager.HasEventBeenTriggered(eventFlag))
        {
            return;
        }

        Debug.Log("Checkpoint Saved: " + gameObject.name);
        currentCheckpoint = this;
        GeneralManager.instance.SetEventFlag(eventFlag);

        playerSpawn = transform.position;
        playerLookDirection = transform.eulerAngles;
    }

    public static Vector3 GetPlayerRespawnPosition()
    {
        if (currentCheckpoint != null)
        {
            Debug.Log(currentCheckpoint.name);
            return currentCheckpoint.transform.position;
        }
        else
        {
            Debug.Log("No Checkpoint");
            return playerSpawn;
        }

        // return playerSpawn;
    }

    public static Vector3 GetPlayerRespawnRotation()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.transform.eulerAngles;
        }
        else
        {
            return playerLookDirection;
        }

        // return playerLookDirection;
    }
}