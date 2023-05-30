using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRising : MonoBehaviour
{
    public GameObject wallMesh;
    public Transform destination;
    public float speed;
    public bool climb = false;

    private void Update()
    {
        if(climb)
        {
            wallMesh.transform.position = Vector3.MoveTowards(wallMesh.transform.position, destination.position, speed * Time.deltaTime);
        }
    }
}
