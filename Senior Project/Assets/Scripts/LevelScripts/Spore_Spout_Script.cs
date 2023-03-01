using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Spore_Spout_Script : TriggerScript
{
    public GameObject spoutEffect;
    private GameObject target;

    public override void ActionToTrigger()
    {
        spoutEffect.GetComponent<VisualEffect>().Play();
    }

    public override void ActionToStop()
    {
        spoutEffect.GetComponent<VisualEffect>().Stop();
    }
}
