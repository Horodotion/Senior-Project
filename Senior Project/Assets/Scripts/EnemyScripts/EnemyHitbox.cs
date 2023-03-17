using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : EnemyController
{
    [Header("Hitbox")]
    public bool critSpot;
    [ToggleableVarable("critSpot", true)] public float critMultiplier;
    public EnemyController enemy;

    public override void OnEnable()
    {

    }

    public override void Damage(float damageAmount, DamageType damageType = DamageType.nuetral)
    {
        if (damageImmunities.Contains(damageType))
        {
            return;
        }

        float damage = DamageCalculation(damageAmount, damageType);

        // StartCoroutine(InvincibilityFrames());

        enemy.health.AddToStat(-damage);
        if (enemy.health.stat <= enemy.health.minimum)
        {
            enemy.CommitDie();
        }
    }

    public override float DamageCalculation(float damage, DamageType damageType)
    {
        if (critSpot)
        {
            damage *= critMultiplier;
        }
        
        if (damageVulnerabilities.Contains(damageType))
        {
            return damage * vulnerabilityMultiplier;
        }
        else if (damageResistances.Contains(damageType))
        {
            return damage * resistanceMultiplier;
        }

        return damage;
    }
}
