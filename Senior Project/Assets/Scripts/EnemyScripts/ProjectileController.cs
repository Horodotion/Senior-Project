using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float force = 2;

    public GameObject destroyEffectPrefab;
    // On fresh prefabs I set health to 10 by default, but feel free to change if we have a global damage scale
    public float explosionRadius, baseDamageDealt, secondsUntilParticlesAreDestroyed;


    private void Update()
    {
        this.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Enemy")
        {
            Explode();
        }
    }
    public void Explode()
    {
        GameObject destructionParticles = Instantiate(destroyEffectPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(destructionParticles, secondsUntilParticlesAreDestroyed);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hC in hitColliders)
        {
            var damage = baseDamageDealt - Vector3.Distance(transform.position, hC.transform.position);
            damage = Mathf.Clamp(baseDamageDealt, 0, damage);

            // INSERT HERE: Function or however damage is assigned, pass each object returned in hitColliders the damage variable above
            if (hC.gameObject.tag == "Player" && hC.GetComponent<PlayerPuppet>() != null)
            {
                hC.GetComponent<PlayerPuppet>().Damage(damage);
            }
            if (hC.gameObject.tag == "Enemy" && hC.GetComponent<EnemyController>() != null && !hC.GetComponent<EnemyController>().dead)
            {
                hC.GetComponent<EnemyController>().Damage(damage);
            }

            // Optional addition: Explosion force equal to damage, originating from this object's position. Remove if not wanted
            if (hC.GetComponent<Rigidbody>() != null)
            {
                hC.GetComponent<Rigidbody>().AddExplosionForce(damage * 5, transform.position, explosionRadius, 1f);
            }
        }
        Destroy(gameObject);
    }
}
