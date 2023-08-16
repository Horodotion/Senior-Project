using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : TriggerScript
{
    public override void ActionToTrigger()
    {
        PlayerController.puppet.CommitDie();
    }
}
