using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocks_Fall_Script : MonoBehaviour
{
    public GameObject Active_Objects;
    public GameObject Inactive_Objects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Inactive_Objects.SetActive(false);
            Active_Objects.SetActive(true);
        }
    }
}
