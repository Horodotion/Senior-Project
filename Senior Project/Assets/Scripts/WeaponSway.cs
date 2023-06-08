using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField]
    private float smooth;

    [SerializeField]
    private float swayMultiplier;

    // Update is called once per frame
    void Update()
    {
        // Get input
        Vector2 sway = PlayerController.instance.lookAxis;

        // Target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-sway.y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(sway.x, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        //Rotate object
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
