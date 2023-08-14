using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRockAnchor_v2 : TriggerScript
{
    public override void ActionToTrigger()
    {
        Destroy(this.gameObject);
    }
}
