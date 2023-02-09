using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public GameObject missilePrefab;
    public float cooldown;
    [HideInInspector] public float timeRemaining;

    void FixedUpdate()
    {
        if (timeRemaining <= 0)
        {
            timeRemaining = cooldown;

            GameObject newMissile = SpawnManager.instance.GetGameObject(missilePrefab, SpawnType.projectile);

            newMissile.transform.position = transform.position;
            newMissile.transform.rotation = transform.rotation;
            newMissile.GetComponent<ProjectileController>().LaunchProjectile();

            newMissile.SetActive(true);
        }
        else
        {
            timeRemaining -= Time.deltaTime;
        }
    }

}
