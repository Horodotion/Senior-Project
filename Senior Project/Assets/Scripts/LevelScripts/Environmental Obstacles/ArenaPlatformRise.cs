using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaPlatformRise : MonoBehaviour
{
    public Transform[] spawnPoints;
    public int currentIndex = 0;
    private Transform currentTransform;
    public GameObject toSpawn;
    private bool playerIn;


    private void Start()
    {
        currentTransform = spawnPoints[currentIndex];    
    }

    private void Update()
    {
        if(playerIn)
        {
            SpawnPlatform(currentTransform);
        }
    }

    private void SpawnPlatform(Transform spawnPoint)
    {
        Instantiate(toSpawn, new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), Quaternion.identity);

        if (currentIndex < spawnPoints.Length)
        {
            currentIndex++;
            currentTransform = spawnPoints[currentIndex];
            SpawnPlatform(currentTransform);
        }

        if (currentIndex == spawnPoints.Length)
        {
            playerIn = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = true; 
        }
    }
}
