using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Enemy_LineOfSightChecker : MonoBehaviour
{
    public SphereCollider sphereCollider;
    //public float fov = 180f;
    public LayerMask losLayers;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
}