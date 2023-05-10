using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextLevelScript : TriggerScript
{
    public static LoadNextLevelScript instance;
    public bool activeLoadingZone = true;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    public override void ActionToTrigger()
    {
        GeneralManager.LoadNextLevel();
    }
}
