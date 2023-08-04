using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRockFeedback : MonoBehaviour
{
    public GameObject smashVFX;
    public Transform vfxSpawnPoint;
    public AudioSource crashSFX;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Log"))
        {
            Instantiate(smashVFX, new Vector3(vfxSpawnPoint.position.x, vfxSpawnPoint.position.y, vfxSpawnPoint.position.z), Quaternion.identity);
            crashSFX.Play();
        }
    }
}
