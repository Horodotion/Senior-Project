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
    public float explosionRadius, baseDamageDealt, secondsUntilParticlesAreDestroyed;


    // Not sure exactly how we're assigning damage from the player controller or wherever,
    // but this should be simple enough to replace with whatever function gets called
    public override void CommitDie()
    {
        if (dead)
        {
            return;
        }       

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
                GameObject destructionParticles = SpawnManager.instance.GetGameObject(destroyEffectPrefab, SpawnType.vfx);
                destructionParticles.transform.position = customTransform.transform.position;
            }
            else
            {
                GameObject destructionParticles = SpawnManager.instance.GetGameObject(destroyEffectPrefab, SpawnType.vfx);
                destructionParticles.transform.position = transform.transform.position;
            }
        }


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hC in hitColliders)
        {
            if (hC.gameObject.tag == "Player" && hC.GetComponent<PlayerPuppet>() != null)
            {
                Debug.Log(hC.gameObject.name);
                hC.GetComponent<PlayerPuppet>().ChangeTemperature(baseDamageDealt);
            }
            if (hC.gameObject.tag == "Enemy" && hC.GetComponent<EnemyController>() != null && !hC.GetComponent<EnemyController>().dead)
            {
                hC.GetComponent<EnemyController>().Damage(baseDamageDealt, hC.ClosestPoint(transform.position), DamageType.nuetral);
            }

        }
        Destroy(this.gameObject);
    }
}
