using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerScript : TriggerScript
{
    public string animTriggerName = "RocksFall";
    public Animator anim;

    void Awake()
    {
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }
        else
        {
            anim = GetComponentInChildren<Animator>();
        }
        
    }

    public override void ActionToTrigger()
    {
        anim.SetTrigger(animTriggerName);
    }
}
