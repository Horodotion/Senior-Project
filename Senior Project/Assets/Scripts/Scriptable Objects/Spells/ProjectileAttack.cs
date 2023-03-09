using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/ProjectileAttack")]
public class ProjectileAttack : Spell
{
    [Header("Projectile Spell Variables")]
    [ToggleableVarable("chargingSpell", true)] public float timeBetweenProjectiles;
    [HideInInspector] public float timeBetweenProjectilesRemaining;

    public override void SpellUpdate()
    {
        base.SpellUpdate();

        if (timeBetweenProjectilesRemaining > 0)
        {
            timeBetweenProjectilesRemaining -= Time.deltaTime;
        }
    }

    public override void Fire()
    {
        if (chargingSpell)
        {
            if (timeBetweenProjectilesRemaining <= 0)
            {
                ProjectileFire();
                timeBetweenProjectilesRemaining = timeBetweenProjectiles;
            }
        }
        else
        {
            ProjectileFire();
        }

        ChangePlayerTemp();
    }
}
