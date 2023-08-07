using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : TriggerScript
{
    public ExplosiveObject ourEnemy;

    public override void ActionToTrigger()
    {
        if (!ourEnemy.dead)
        {
            ourEnemy.CommitDie();
        }
    }
}
