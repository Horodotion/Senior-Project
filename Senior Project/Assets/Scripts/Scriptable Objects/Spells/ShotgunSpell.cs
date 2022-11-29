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
        Vector3 pos = GetFirePos().TransformPoint(Vector3.zero);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 shotDirection = Accuracy(playerCameraTransform.forward, spreadAngle);

            if (objectToSpawn != null)
            {
                // GameObject iceParticle = Instantiate(objectToSpawn, pos, Quaternion.LookRotation(shotDirection, Vector3.up));
                GameObject iceParticle = SpawnManager.instance.GetGameObject(objectToSpawn, SpawnType.projectile);
                iceParticle.transform.position = pos;
                iceParticle.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.up);

                iceParticle.GetComponent<ProjectileController>().LaunchProjectile();
            }
        }
    }

    public virtual void DamageEnemy(EnemyController enemyController, float pelletsThatHit)
    {
        enemyController.Damage((damage) * pelletsThatHit);
    }
}
