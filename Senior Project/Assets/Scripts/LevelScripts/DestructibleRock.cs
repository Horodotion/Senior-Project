using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleRock : MonoBehaviour
{
    public GameObject smashVFX;
    public Transform vfxSpawnPoint;
    public GameObject toSmash;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Log"))
        {
            Instantiate(smashVFX, new Vector3(vfxSpawnPoint.position.x, vfxSpawnPoint.position.y, vfxSpawnPoint.position.z), Quaternion.identity);
            Invoke(nameof(Smash), 0.5f);
        }
    }

    public void Smash()
    {
        Debug.Log("yep");
        Destroy(toSmash.gameObject);
    }
}
