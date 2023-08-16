using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collectible))]
public class DestructableCollectible : EnemyController
{
    public Collectible ourCollectible;

    void Start()
    {
        ourCollectible = GetComponent<Collectible>();
    }

    public override void CommitDie()
    {
        base.CommitDie();
        ourCollectible.Collect();
    }
}
