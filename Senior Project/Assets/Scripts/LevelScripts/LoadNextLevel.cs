using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextLevel : TriggerScript
{
    public override void ActionToTrigger()
    {
        GeneralManager.LoadNextLevel();
    }
}
