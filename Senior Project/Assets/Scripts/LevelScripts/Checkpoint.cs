using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : TriggerScript
{
    public EventFlag eventFlag;

    public static Vector3 playerSpawn;
    public static bool originalSpawnLocated;
    public static Vector3 playerLookDirection;

    void Start()
    {
        if (!originalSpawnLocated && PlayerController.puppet != null)
        {
            originalSpawnLocated = true;
            playerSpawn = PlayerController.puppet.transform.position;
            playerLookDirection = PlayerController.puppet.transform.localEulerAngles;
        }

        GeneralManager.instance.AddEventToDict(eventFlag);
        if (GeneralManager.instance.eventFlags[eventFlag.eventID].eventTriggered)
        {
            gameObject.SetActive(false);
        }
    }

    public override void ActionToTrigger()
    {
        if (GeneralManager.instance.eventFlags.ContainsKey(eventFlag.eventID) && !GeneralManager.instance.eventFlags[eventFlag.eventID].eventTriggered)
        {
            if (!GeneralManager.instance.eventFlags[eventFlag.eventID].eventTriggered)
            {
                GeneralManager.instance.SetEventFlag(eventFlag.eventID);

                Debug.Log(PlayerController.puppet.transform.position);
                playerSpawn = PlayerController.puppet.transform.position;
                playerLookDirection = new Vector3(transform.forward.x, transform.forward.y, 0);
            }
        }
        else
        {
            Debug.Log(PlayerController.puppet.transform.position);
            playerSpawn = PlayerController.puppet.transform.position;
            playerLookDirection = new Vector3(transform.forward.x, transform.forward.y, 0);
        }
    }
}