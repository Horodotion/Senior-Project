using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalHazardScript : TriggerScript
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
        if (playerInArea)
        {
            PlayerController.puppet.ChangeTemperature(temperaturePerSecond * Time.deltaTime);

            if (PlayerController.instance.temperature.stat >= PlayerController.instance.temperature.maximum ||
                PlayerController.instance.temperature.stat <= PlayerController.instance.temperature.minimum)
            {
                playerInArea = false;
            }
        }
    }
}
