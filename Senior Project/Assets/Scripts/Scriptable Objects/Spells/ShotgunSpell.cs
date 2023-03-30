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

    public override void Fire()
    {
        ProjectileFire();
        ChangePlayerTemp();
    }

    public override void ProjectileFire()
    {
        Vector3 pos = GetFirePos().position;
        Vector3 targetDirection = FindTargetLocation() - pos;

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 shotDirection = Accuracy(targetDirection.normalized, spreadAngle);

            if (objectToSpawn != null)
            {
                GameObject iceParticle = SpawnManager.instance.GetGameObject(objectToSpawn, SpawnType.projectile);
                iceParticle.transform.position = pos;
                iceParticle.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.up);

                ProjectileController newProjectileScript = iceParticle.GetComponent<ProjectileController>();

                newProjectileScript.damage = damage;
                newProjectileScript.damageType = damageType;
                newProjectileScript.hostileFaction = Faction.Enemy;
                newProjectileScript.LaunchProjectile();
            }
        }
    }

    public virtual void DamageEnemy(EnemyController enemyController, float pelletsThatHit)
    {
        enemyController.Damage((damage) * pelletsThatHit, damageType);
    }
}
