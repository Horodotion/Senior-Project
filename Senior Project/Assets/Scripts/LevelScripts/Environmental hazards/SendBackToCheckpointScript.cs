using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendBackToCheckpointScript : TriggerScript
{
    public override void ActionToTrigger()
    {
        GeneralManager.LoadCheckPoint();
    }
}
