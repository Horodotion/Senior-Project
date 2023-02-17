using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : EnemyController
{
    public ProjectileController ourProjectile;
    public DamageType damageImmunity;

    public override void Damage(float damageAmount, DamageType damageType)
    {
        if (damageType == damageImmunity)
        {
            return;
        }

        base.Damage(damageAmount, damageType);
    }

    public override void CommitDie()
    {
        dead = true;
        Debug.Log(ourProjectile.gameObject.name + " is dead");
        ourProjectile.Explode();
    }
}
