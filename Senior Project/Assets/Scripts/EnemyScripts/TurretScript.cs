using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : EnemyController
{
    public enum TurretType
    {
        projectile,
        beam
    }

    [Header("Turret Components")]
    public Transform turretHinge;

    public TurretType turretType;
    public GameObject iceProjectile;
    public GameObject fireProjectile;
    public GameObject iceBeam;
    public GameObject fireBeam;

    [Header("Turret Attacks")]
    public float rotationSpeed;
    public float damage;
    public float detectionRadius;
    public float attackRange;
    [HideInInspector] public Vector3 beamPos;

    public float attackSpeed;
    [HideInInspector] public float attackTimer;
    

    public void Awake()
    {
        if (turretType == TurretType.beam)
        {
            iceBeam = Instantiate(iceBeam, turretHinge.position, turretHinge.rotation);
            iceBeam.transform.SetParent(turretHinge);

            fireBeam = Instantiate(fireBeam, turretHinge.position, turretHinge.rotation);
            fireBeam.transform.SetParent(turretHinge);
        }
    }

    public void FixedUpdate()
    {
        if (Vector3.Distance(PlayerController.puppet.GetComponent<Collider>().bounds.center, turretHinge.position) > detectionRadius)
        {
            Idle();
            return;
        }


        if (turretType == TurretType.beam)
        {
            AttackWithBeam();
        }
        else if (turretType == TurretType.projectile)
        {
            AttackWithProjectiles();
        }
    }

    public override void CommitDie()
    {
        base.CommitDie();

        MobSpawnerController.instance.DestroyObjectInMobSpawrer(this.gameObject);

        gameObject.SetActive(false);
    }

    public void Idle()
    {
        if (turretType == TurretType.beam)
        {
            if (fireBeam.activeInHierarchy)
            {
                fireBeam.SetActive(false);
            }

            if (iceBeam.activeInHierarchy)
            {
                iceBeam.SetActive(false);
            }
        }
    }

    public void AttackWithBeam()
    {
        RaycastHit hit;
        Vector3 direction = PlayerController.puppet.GetComponent<Collider>().bounds.center - turretHinge.position;
        float damage = this.damage * Time.deltaTime * Mathf.Sign(PlayerController.instance.temperature.stat);

        Vector3 rotationDirection = Vector3.RotateTowards(turretHinge.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);
        turretHinge.rotation = Quaternion.LookRotation(rotationDirection);

        if (Physics.Raycast(turretHinge.position, turretHinge.forward, out hit, attackRange, -1, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.GetComponent<PlayerPuppet>() != null)
            {
                hit.collider.gameObject.GetComponent<PlayerPuppet>().ChangeTemperature(damage);
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

    public void AttackWithProjectiles()
    {
        Vector3 direction = PlayerController.puppet.GetComponent<Collider>().bounds.center - turretHinge.position;

        Vector3 rotationDirection = Vector3.RotateTowards(turretHinge.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);
        turretHinge.rotation = Quaternion.LookRotation(rotationDirection);

        

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackTimer = attackSpeed;

            GameObject newProjectile = null;
            ProjectileController newProjectileScript = null;
            if (Mathf.Sign(PlayerController.instance.temperature.stat) >= 0f)
            {
                newProjectile = SpawnManager.instance.GetGameObject(fireProjectile, SpawnType.projectile);
                newProjectileScript = newProjectile.GetComponent<ProjectileController>();

                newProjectileScript.damage = damage;
                newProjectileScript.damageType = DamageType.fire;
            }
            else
            {
                newProjectile = SpawnManager.instance.GetGameObject(iceProjectile, SpawnType.projectile);
                newProjectileScript = newProjectile.GetComponent<ProjectileController>();

                newProjectileScript.damage = damage * -1f;
                newProjectileScript.damageType = DamageType.ice;
            }

            newProjectile.transform.position = turretHinge.position;
            newProjectile.transform.rotation = turretHinge.rotation;
            newProjectileScript.hostileFaction = Faction.Player;
            newProjectileScript.LaunchProjectile();
        }
    }
}
