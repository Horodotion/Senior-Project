using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public List<GameObject> allProjectiles;
    public GameObject projectileToSpawn;


    public GameObject GetProjectile()
    {
        GameObject projectileToReturn = null;
        bool projectileExists = false;

        for (int i = 0; i > allProjectiles.Count; i++)
        {
            if (allProjectiles[i].activeSelf == false)
            {
                projectileExists = true;
                projectileToReturn = allProjectiles[i];
                projectileToReturn.SetActive(true);
                break;
            }
        }

        if (!projectileExists)
        {
            projectileToReturn = Instantiate(projectileToSpawn);
            allProjectiles.Add(projectileToReturn);
            // projectileToReturn.GetComponent<ProjectileController>().ourProjectileManager = this;
        }

        return projectileToReturn;

    }
}
