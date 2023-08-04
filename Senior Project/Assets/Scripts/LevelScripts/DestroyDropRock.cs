using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDropRock : MonoBehaviour
{
    //public GameObject smashVFX;
    //public Transform vfxSpawnPoint;
    //public AudioSource crashSFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Log"))
        {
            Destroy(other.gameObject);
            //crashSFX.Play();
        }
    }
}

