using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObject : EnemyController
{
    public GameObject destroyEffectPrefab;
    //GameObject to help in case the model does not have perfect transforms

    //put an empty game object in the following variable so the explosion effect can find it - Matt
    public GameObject customTransform;
    
    // On fresh prefabs I set health to 10 by default, but feel free to change if we have a global damage scale
    public float explosionRadius, baseDamageDealt, baseTempChange, secondsUntilParticlesAreDestroyed;


    // Not sure exactly how we're assigning damage from the player controller or wherever,
    // but this should be simple enough to replace with whatever function gets called
    public override void CommitDie()
    {
        base.CommitDie();
        Explode();
    }

    public override void Explode()
    {
        if (destroyEffectPrefab != null)
        {
            // Checks if there is a custom transform - Matt
            if (customTransform != null)
            {
                // If there is use the custom transform to spawn the particles - Matt
                GameObject destructionParticles = Instantiate(destroyEffectPrefab, customTransform.transform.position, Quaternion.Euler(0, 0, 0));
                // Destroy(destructionParticles, secondsUntilParticlesAreDestroyed);
            }
            else
            {
                GameObject destructionParticles = Instantiate(destroyEffectPrefab, transform.position, Quaternion.Euler(0, 0, 0));
                // Destroy(destructionParticles, secondsUntilParticlesAreDestroyed);
            }
        }


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hC in hitColliders)
        {
            float damageFallOff = Mathf.Cos(Vector3.Distance(transform.position, hC.transform.position) / explosionRadius); 
            float explosionDamage = Mathf.Clamp(damageFallOff * baseDamageDealt, 0, baseDamageDealt);

            // INSERT HERE: Function or however damage is assigned, pass each object returned in hitColliders the damage variable above
            if (hC.gameObject.tag == "Player" && hC.GetComponent<PlayerPuppet>() != null)
            {
                hC.GetComponent<PlayerPuppet>().ChangeTemperature(explosionDamage);
            }
            if (hC.gameObject.tag == "Enemy" && hC.GetComponent<EnemyController>() != null && !hC.GetComponent<EnemyController>().dead)
            {
                hC.GetComponent<EnemyController>().Damage(explosionDamage, DamageType.nuetral);
            }

        }
        Destroy(this.gameObject);
    }
}
