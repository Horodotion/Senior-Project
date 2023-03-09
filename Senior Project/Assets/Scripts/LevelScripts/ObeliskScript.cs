using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskScript : TriggerScript
{
    public float temperaturePerSecond;
    [HideInInspector] public bool playerInArea;

    public override void ActionToTrigger()
    {
        playerInArea = true;
        Debug.Log("Player In Area");
    }

    public override void ActionToStop()
    {
        playerInArea = false;
        Debug.Log("Player out of Area");
    }

    void FixedUpdate()
    {
        if (playerInArea && PlayerController.instance.temperature.stat != 0)
        {
            float tempPerSecond = temperaturePerSecond * Time.deltaTime;

            if (Mathf.Abs(PlayerController.instance.temperature.stat) >= tempPerSecond)
            {
                PlayerController.puppet.ChangeTemperature(-Mathf.Sign(PlayerController.instance.temperature.stat) * tempPerSecond);
            }
            else
            {
                PlayerController.puppet.ChangeTemperature(-PlayerController.instance.temperature.stat);
            }
        }
    }
}