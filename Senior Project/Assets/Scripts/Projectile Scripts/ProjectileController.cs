using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{   
    [Header("Projectile Variables")]
    [HideInInspector] public Rigidbody rb;
    public Faction hostileFaction;
    public float projectileSpeed;
    public float lifeSpan;
    public float range;
    [HideInInspector] public Vector3 origin;
    [HideInInspector] public Quaternion rotation;
    public DamageType damageType;
    public float damage;
    [HideInInspector] public bool launched = false;
    

    public GameObject destroyEffectPrefab;
    // On fresh prefabs I set health to 10 by default, but feel free to change if we have a global damage scale
    // public float explosionRadius, baseDamageDealt, secondsUntilParticlesAreDestroyed;

    public virtual void Awake()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log(gameObject.name + " needs a rigidbody");
            gameObject.SetActive(false);
        }
    }

    public virtual void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, origin) >= range)
        {
            Deactivate();
        }
    }

    public virtual void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != hostileFaction.ToString())
        {
            return;
        }
        Debug.Log(hostileFaction == Faction.Player);
        if (hostileFaction == Faction.Enemy && col.gameObject.GetComponent<EnemyController>() != null)
        {
            col.gameObject.GetComponent<EnemyController>().Damage(damage, damageType);
        }
        else if (hostileFaction == Faction.Player && col.gameObject.GetComponent<PlayerPuppet>() != null)
        {
            col.gameObject.GetComponent<PlayerPuppet>().ChangeTemperature(damageToTemperature() * damage);
        }

        Deactivate();
    }

    public virtual void Explode()
    {
        Deactivate();
    }

    public virtual void LaunchProjectile()
    {
        //origin = gameObject.transform.position;
        gameObject.transform.position = origin;
        gameObject.transform.rotation = rotation;
        rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
        launched = true;
    }

    public virtual void Deactivate()
    {
        rb.velocity = Vector3.zero;
        launched = false;
        lifeSpan = 0;
        gameObject.SetActive(false);
    }

    public float damageToTemperature()
    {
        float i = 1;

        if (damageType == DamageType.ice)
        {
            i = -1;
        }

        return i;
    }
}
