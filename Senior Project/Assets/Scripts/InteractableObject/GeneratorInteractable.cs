using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorInteractable : LockInteractable
{
    public GameObject keyHoleGameObject;

    public override void Unlock()
    {
        base.Unlock();
        keyHoleGameObject.SetActive(true);
    }
}
