using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testwhirl : MonoBehaviour
{
    public FreezingPlatforms lilyPad;

    [SerializeField] float spinSpeed = 30f;
    [SerializeField] float deadSpeed = 15f;
    [SerializeField] Vector3 _axis = Vector3.forward;
    void Update()
    {
        if (lilyPad.isDead != true)
        {
            transform.Rotate(_axis.normalized * spinSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(_axis.normalized * deadSpeed * Time.deltaTime);
        }
    }
}

