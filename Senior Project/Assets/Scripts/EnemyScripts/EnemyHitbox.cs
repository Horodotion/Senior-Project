using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : EnemyController
{
    [Header("Hitbox")]

    public bool critSpot;
    public EnemyController enemy;
    public EnemyHitbox hitBox;

    public override void Damage(float damageAmount, DamageType damageType = DamageType.nuetral)
    {
        enemy.Damage(damageAmount, damageType);
    }
}
