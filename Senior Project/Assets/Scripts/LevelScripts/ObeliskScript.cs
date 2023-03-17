using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskScript : TriggerScript
{
    public float temperaturePerSecond;
    [HideInInspector] public bool playerInArea;

    public bool hasMaxCharge;
    public IndividualStat chargeAmount;

    public bool hasReserves;
    public float reserveAmount;
    public bool isActive;
    public float minimumChargeToActivate;
    public float rechargeRate;

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
            ChangePlayerTemp();
        }
        else if (!playerInArea && hasMaxCharge)
        {
            RechargeObelisk();
        }
    }

    public void ChangePlayerTemp()
    {
        if (hasMaxCharge && (chargeAmount.stat <= 0 || !isActive))
        {
            return;
        }

        float tempPerSecond = temperaturePerSecond * Time.deltaTime;

        if (Mathf.Abs(PlayerController.instance.temperature.stat) >= tempPerSecond)
        {
            PlayerController.puppet.ChangeTemperature(-Mathf.Sign(PlayerController.instance.temperature.stat) * tempPerSecond);
            ReduceCharge(tempPerSecond);
        }
        else
        {
            PlayerController.puppet.ChangeTemperature(-PlayerController.instance.temperature.stat);
            ReduceCharge(PlayerController.instance.temperature.stat);
        }


    }

    public void ReduceCharge(float amount)
    {
        if (hasMaxCharge)
        {
            chargeAmount.stat -= amount;
            if (chargeAmount.stat <= 0)
            {
                isActive = false;
            }
        }
    }

    public void RechargeObelisk()
    {
        if (chargeAmount.stat < chargeAmount.maximum)
        {
            float rechargeAmount = rechargeRate * Time.deltaTime;
            float theoreticalChange = chargeAmount.stat + rechargeAmount;

            if (theoreticalChange >= chargeAmount.maximum)
            {
                rechargeAmount = chargeAmount.maximum - chargeAmount.stat;
            }
            else if (theoreticalChange > reserveAmount)
            {
                rechargeAmount = reserveAmount - rechargeAmount;
            }

            chargeAmount.stat += rechargeAmount;
            reserveAmount -= rechargeAmount;

            if (!isActive && chargeAmount.stat >= minimumChargeToActivate)
            {
                isActive = true;
            }
        }
    }
}