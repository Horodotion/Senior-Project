using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public LayerMask ourLayer;

    void OnMouseEnter()
    {
        gameObject.layer = ourLayer.value;

        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = ourLayer.value;
        }
    }

    void OnMouseExit()
    {
        gameObject.layer = 0;

        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = 0;
        }
    }
}
