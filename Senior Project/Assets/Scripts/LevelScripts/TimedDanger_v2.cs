using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDanger_v2 : TriggerScript
{
    private bool activated;
    [HideInInspector] public bool playerInArea;
    private bool isOn = false;
    public float delay;

    // Timer
    [SerializeField] private float activeTime = 2.5f, inactiveTime = 2.5f;
    private float timer;

    // Temperature affectation
    [SerializeField] private float temperaturePerSecond;
    [SerializeField] private BoxCollider myCollider;

    // Aesthetic
    [SerializeField] private GameObject myParticles;
    public Transform poofSpawnPoint;

    private void Start()
    {
        Invoke(nameof(TurnOn), delay);
    }


    void FixedUpdate()
    {
        
        if(isOn)
        {
            timer -= Time.deltaTime;

            if (timer <= 0) /// Called only one frame
            {
                if (activated)
                {
                    // All effects that happen when the object deactivates
                    //myParticles.SetActive(false);

                    // Reset timer and toggle activity state
                    timer = inactiveTime;
                    activated = false;
                }
                else
                {
                    // All effects that happen when the object activates
                    //myParticles.SetActive(true);
                    Instantiate(myParticles, poofSpawnPoint.transform.position, poofSpawnPoint.transform.rotation);
                    // Reset timer and toggle activity state
                    timer = activeTime;
                    activated = true;
                }
            }
        }
        // Count timer down- when it reaches zero, toggle activity and reset timer

        

        if (playerInArea && activated)
        {
            PlayerController.puppet.ChangeTemperature(temperaturePerSecond * Time.deltaTime);

            if (PlayerController.instance.temperature.stat >= PlayerController.instance.temperature.maximum ||
                PlayerController.instance.temperature.stat <= PlayerController.instance.temperature.minimum)
            {
                playerInArea = false;
            }
        }
    }

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

    private void TurnOn()
    {
        isOn = true;
    }
}
