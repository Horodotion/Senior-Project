using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingProjectile : ProjectileController
{
    [HideInInspector] public Vector3 originalScale;
    public float growthRate;

    public override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
    }

    public override void FixedUpdate()
    {
        float newGrowth = growthRate * Time.deltaTime;

        transform.localScale += new Vector3(newGrowth, newGrowth, newGrowth);

        base.FixedUpdate();
    }

    public override void LaunchProjectile()
    {

        base.LaunchProjectile();
    }

    public override void Deactivate()
    {
        transform.localScale = originalScale;
        base.Deactivate();
    }
}
