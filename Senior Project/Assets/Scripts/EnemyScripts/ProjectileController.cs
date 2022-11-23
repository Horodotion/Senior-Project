using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    public Faction hostileFaction;
    public float projectileSpeed;
    public float lifeSpan;
    public float range;
    [HideInInspector] public Vector3 origin;
    public float damage;

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
        if (col.gameObject.tag == "Enemy" && col.gameObject.GetComponent<EnemyController>() != null)
        {
            col.gameObject.GetComponent<EnemyController>().Damage(damage);
        }
    }

    // public virtual void Explode()
    // {
    //     GameObject destructionParticles = Instantiate(destroyEffectPrefab, transform.position, Quaternion.Euler(0, 0, 0));
    //     Destroy(destructionParticles, secondsUntilParticlesAreDestroyed);

    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
    //     foreach (var hC in hitColliders)
    //     {
    //         var damage = baseDamageDealt - Vector3.Distance(transform.position, hC.transform.position);
    //         damage = Mathf.Clamp(baseDamageDealt, 0, damage);

    //         // INSERT HERE: Function or however damage is assigned, pass each object returned in hitColliders the damage variable above
    //         if (hC.gameObject.tag == "Player" && hC.GetComponent<PlayerPuppet>() != null)
    //         {
    //             hC.GetComponent<PlayerPuppet>().Damage(damage);
    //         }
    //         if (hC.gameObject.tag == "Enemy" && hC.GetComponent<EnemyController>() != null && !hC.GetComponent<EnemyController>().dead)
    //         {
    //             hC.GetComponent<EnemyController>().Damage(damage);
    //         }

    //         // Optional addition: Explosion force equal to damage, originating from this object's position. Remove if not wanted
    //         if (hC.GetComponent<Rigidbody>() != null)
    //         {
    //             hC.GetComponent<Rigidbody>().AddExplosionForce(damage * 5, transform.position, explosionRadius, 1f);
    //         }
    //     }
    //     Destroy(gameObject);
    // }

    public virtual void LaunchProjectile()
    {
        origin = gameObject.transform.position;
        rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
    }

    public virtual void Deactivate()
    {
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
