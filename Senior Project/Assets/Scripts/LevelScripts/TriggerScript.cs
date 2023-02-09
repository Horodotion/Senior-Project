using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerScript : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == PlayerController.puppet.gameObject)
        {
            ActionToTrigger();
        }
    }

    public virtual void OnTriggerExit(Collider col)
    {
        if (col.gameObject == PlayerController.puppet.gameObject)
        {
            ActionToStop();
        }
    }

    public virtual void ActionToTrigger()
    {

    }

    public virtual void ActionToStop()
    {

    }
}
