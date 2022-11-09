using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingProjectile : ProjectileController
{
    public float growthRate;

    void FixedUpdate()
    {
        float newGrowth = growthRate * Time.deltaTime;

        transform.localScale += new Vector3(newGrowth, newGrowth, newGrowth);
    }
}
