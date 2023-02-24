using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : EnemyController
{
    public ProjectileController ourProjectile;

    public override void CommitDie()
    {
        dead = true;
        Debug.Log(ourProjectile.gameObject.name + " is dead");
        ourProjectile.Explode();
    }
}
