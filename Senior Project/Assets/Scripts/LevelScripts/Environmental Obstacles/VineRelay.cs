using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineRelay : EnemyHitbox
{
    public override void CommitDie()
    {
        base.CommitDie();
        enemy.enemyHitboxes.Remove(this);
        enemy.Damage(0, Vector3.zero);
        Destroy(gameObject);
    }

    public override void Damage(float damageAmount, Vector3 hitPosition, DamageType damageType = DamageType.nuetral)
    {
        if (damageImmunities.Contains(damageType))
        {
            return;
        }

        float damage = DamageCalculation(damageAmount, damageType);

        // StartCoroutine(InvincibilityFrames());

        health.AddToStat(-damage);
        if (health.stat <= health.minimum && !dead)
        {
            CommitDie();
        }
    }

    public override void OnEnable()
    {

    }
}

