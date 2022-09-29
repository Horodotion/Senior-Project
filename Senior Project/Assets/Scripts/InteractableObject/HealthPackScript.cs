using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackScript : MonoBehaviour
{
    // The float that determines how much the health pack heals
    public float healValue;

    // Triggers when it collides with something
    private void OnTriggerEnter(Collider other)
    {
        //Checks for tag on whatever collided with it
        if (other.gameObject.tag == "Player")
        {
            //Adds health to Stats
            PlayerController.instance.playerStats.AddToStat(StatType.health, healValue);
            // Calls the placeholder method to update the UI in health. Probably a placeholder
            // UIFunctionsScript.instance.UpdateHealth();
            //SELF DESTRUCT ACTIVATED
            Destroy(gameObject);
        }
    }
}
