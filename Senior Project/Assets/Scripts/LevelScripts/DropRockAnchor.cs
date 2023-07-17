using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRockAnchor : EnemyController
{
    public override void Damage(float damageAmount, Vector3 hitPosition, DamageType damageType = DamageType.nuetral)
    {
        if (enemyHitboxes.Count <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
