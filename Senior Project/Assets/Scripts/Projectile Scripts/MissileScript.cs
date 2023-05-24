using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : ProjectileController
{
    [Header("Missile")]
    public Transform target;
    public float rotationSpeed;
    public float explosionRadius;

    public override void LaunchProjectile()
    {
        base.LaunchProjectile();
        
        if(target == null)
        {
            target = FindNewTarget();
        }
    }

    public override void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, origin) >= range)
        {
            Deactivate();
        }

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            Quaternion directionToMove = Quaternion.Lerp(Quaternion.LookRotation(direction, transform.up), transform.rotation, Time.deltaTime * rotationSpeed);
            
            rb.MoveRotation(directionToMove); //Quaternion.LookRotation(direction, transform.up));
            rb.AddForce(transform.forward * projectileSpeed);
            lifeSpan += rb.velocity.magnitude * Time.fixedDeltaTime;
        }
    }

    public override void OnTriggerEnter(Collider col)
    {
        if (col.isTrigger)
        {
            return;
        }

        Explode();
    }

    public override void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hC in hitColliders)
        {
            float damageFallOff = Mathf.Cos(Vector3.Distance(transform.position, hC.transform.position) / explosionRadius); 
            float explosionDamage = Mathf.Clamp(damageFallOff * damage, 0, damage);

            // INSERT HERE: Function or however damage is assigned, pass each object returned in hitColliders the damage variable above
            if (hC.gameObject.tag == "Player" && hC.GetComponent<PlayerPuppet>() != null)
            {
                hC.GetComponent<PlayerPuppet>().ChangeTemperature(explosionDamage);
            }
            if (hC.gameObject.tag == "Enemy" && hC.GetComponent<EnemyController>() != null && !hC.GetComponent<EnemyController>().dead)
            {
                hC.GetComponent<EnemyController>().Damage(explosionDamage, Vector3.zero, damageType);
            }

        }

        Deactivate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public Transform FindNewTarget()
    {
        Collider[] allColliders = Physics.OverlapSphere(transform.position, range);
        List<Transform> allTargets = new List<Transform>();

        foreach (Collider col in allColliders)
        {
            if (col.gameObject.tag == hostileFaction.ToString())
            {
                allTargets.Add(col.gameObject.transform);
            }
        }

        if (allTargets.Count == 0)
        {
            Debug.Log("No target found");
            return null;
        }

        if (allTargets.Count == 1)
        {
            Debug.Log("One target found");
            return allTargets[0];
        }

        int targetID = 0;
        float targetDistance = Mathf.Infinity;
        for (int i = 0; i > allTargets.Count; i++)
        {
            if (Vector3.Distance(allTargets[i].position, transform.position) < targetDistance)
            {
                targetID = i;
                targetDistance = Vector3.Distance(allTargets[i].position, transform.position);
            }
        }
        return allTargets[targetID];
    }
}
