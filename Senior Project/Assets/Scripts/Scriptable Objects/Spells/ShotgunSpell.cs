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

    public override void InitializeSpell()
    {
        base.InitializeSpell();

        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.InitializeIcicles(this);
        }
    }

    public override void SecondarySpellUpdate(float timeHolder)
    {
        base.SecondarySpellUpdate(timeHolder);

        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.ChangeIcicleCounter(this);
        }
    }

    public override void ProjectileFire()
    {
        Vector3 pos = GetFirePos().position;
        Vector3 targetDirection = FindTargetLocation() - pos;

        if (GetFirePos().GetComponent<AudioSource>() != null)
        {
            GetFirePos().GetComponent<AudioSource>().Play();
        }

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 shotDirection = Accuracy(targetDirection.normalized, spreadAngle);

            if (objectToSpawn != null)
            {
                GameObject iceParticle = SpawnManager.instance.GetGameObject(objectToSpawn, SpawnType.projectile);
                iceParticle.transform.position = pos;
                iceParticle.transform.rotation = Quaternion.LookRotation(shotDirection, Vector3.up);

                ProjectileController newProjectileScript = iceParticle.GetComponent<ProjectileController>();

                newProjectileScript.damage = AssignDamage();
                newProjectileScript.damageType = damageType;
                newProjectileScript.hostileFaction = Faction.Enemy;
                newProjectileScript.LaunchProjectile();
            }
        }
    }
}
