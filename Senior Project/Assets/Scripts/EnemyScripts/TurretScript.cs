using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : EnemyController
{

    [Header("Turret Variables")]
    public Transform turretHinge;
    public GameObject iceBeam;
    public GameObject fireBeam;

    public float rotationSpeed;
    public float damagePerSecond;
    public float detectionRadius;
    public float attackRange;
    [HideInInspector] public Vector3 beamPos;

    public void Awake()
    {
        iceBeam = Instantiate(iceBeam, turretHinge.position, turretHinge.rotation);
        iceBeam.transform.SetParent(turretHinge);

        fireBeam = Instantiate(fireBeam, turretHinge.position, turretHinge.rotation);
        fireBeam.transform.SetParent(turretHinge);
    }

    public void FixedUpdate()
    {
        if (Vector3.Distance(PlayerController.puppet.GetComponent<Collider>().bounds.center, turretHinge.position) > detectionRadius)
        {
            if (fireBeam.activeInHierarchy)
            {
                fireBeam.SetActive(false);
            }

            if (iceBeam.activeInHierarchy)
            {
                iceBeam.SetActive(false);
            }

            return;
        }

        RaycastHit hit;
        Vector3 direction = PlayerController.puppet.GetComponent<Collider>().bounds.center - turretHinge.position;
        float damage = damagePerSecond * Time.deltaTime * Mathf.Sign(PlayerController.instance.temperature.stat);

        Vector3 rotationDirection = Vector3.RotateTowards(turretHinge.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);
        turretHinge.rotation = Quaternion.LookRotation(rotationDirection);

        if (Physics.Raycast(turretHinge.position, turretHinge.forward, out hit, attackRange))//, -1, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.GetComponent<PlayerPuppet>() != null)
            {
                
                hit.collider.gameObject.GetComponent<PlayerPuppet>().ChangeTemperature(damage);
                Debug.Log(damage);
            }

            beamPos = hit.point;
        }
        else
        {
            beamPos = turretHinge.position + (turretHinge.forward * attackRange);
        }

        if (damage >= 0)
        {
            if (!fireBeam.activeInHierarchy)
            {
                fireBeam.SetActive(true);
            }

            if (iceBeam.activeInHierarchy)
            {
                iceBeam.SetActive(false);
            }

            fireBeam.GetComponent<BeamScript>().ChangeEndPosition(beamPos);
        }
        else if (damage < 0)
        {
            if (fireBeam.activeInHierarchy)
            {
                fireBeam.SetActive(false);
            }

            if (!iceBeam.activeInHierarchy)
            {
                iceBeam.SetActive(true);
            }

            iceBeam.GetComponent<BeamScript>().ChangeEndPosition(beamPos);
        }
    }

    public override void CommitDie()
    {
        base.CommitDie();

        gameObject.SetActive(false);
    }
}
