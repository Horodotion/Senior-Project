using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineRelay : EnemyController
{
    [Header("References")]
    public LogColliders logScript;

    public override void CommitDie()
    {
        base.CommitDie();
        logScript.vinesLeft = logScript.vinesLeft - 1;
        Destroy(gameObject);
    }


}

