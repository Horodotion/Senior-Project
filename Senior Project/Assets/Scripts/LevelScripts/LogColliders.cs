using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogColliders : EnemyController
{
    public GameObject boxCol;
    public GameObject meshCol;
    public int vinesLeft;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCol.SetActive(false);
    }

    public override void Damage(float damageAmount, Vector3 hitPosition, DamageType damageType = DamageType.nuetral)
    {
        if(enemyHitboxes.Count <= 0)
        {
            meshCol.SetActive(false);
            boxCol.SetActive(true);
        }
    }
}
