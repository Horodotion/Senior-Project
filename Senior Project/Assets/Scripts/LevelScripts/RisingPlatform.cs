using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingPlatform : EnemyController
{
    public Transform target;
    public float speed;
    
    public void Update()
    {
        if (enemyHitboxes.Count <= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
