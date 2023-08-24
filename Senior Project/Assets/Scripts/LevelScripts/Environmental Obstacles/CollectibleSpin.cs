using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpin : MonoBehaviour
{

    [SerializeField] float spinSpeed = 30f;
    [SerializeField] Vector3 _axis = Vector3.forward;
    void FixedUpdate()
    {
        transform.Rotate(_axis.normalized * spinSpeed * Time.deltaTime);

    }
}
