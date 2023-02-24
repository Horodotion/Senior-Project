using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/Shotgun")]
public class ShotgunSpell : Spell
{

    [Header("Shotgun Variables")]
    public float pelletsPerShot;
    public float minimumSpreadRange;
    public float spreadAngle;

    public override void SecondarySpellUpdate()
    {
        if (charges < maximumCharges)
        {
            if (rechargeTimer <= 0)
            {
                charges++;
                rechargeTimer = rechargeRate;
            }
            else
            {
                rechargeTimer -= Time.deltaTime;
            }
        }
    }

    public override void Fire()
    {
        ProjectileFire();
        ChangePlayerTemp();
    }

    public override void ProjectileFire()
    {
        Vector3 pos = GetFirePos().position;
        // Debug.Log(pos);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 shotDirection = Accuracy(playerCameraTransform.forward, spreadAngle);

            if (objectToSpawn != null)
            {
                // GameObject iceParticle = Instantiate(objectToSpawn, pos, Quaternion.LookRotation(shotDirection, Vector3.up));
                GameObject iceParticle = SpawnManager.instance.GetGameObject(objectToSpawn, SpawnType.projectile);
                Debug.Log(pos);
                iceParticle.transform.position = pos;
                iceParticle.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.up);

                iceParticle.GetComponent<ProjectileController>().LaunchProjectile();
            }
        }
    }

    public virtual void DamageEnemy(EnemyController enemyController, float pelletsThatHit)
    {
        enemyController.Damage((damage) * pelletsThatHit, damageType);
    }
}
