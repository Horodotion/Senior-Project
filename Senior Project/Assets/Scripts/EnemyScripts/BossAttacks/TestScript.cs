using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Test");
    }
}
