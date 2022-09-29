using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName = "Weapon/Shotgun")]

public class Shotgun : Weapon
{
    public float pelletsPerShot, minimumSpreadRange, spreadAngle;

    public override void HitScanFire()
    {
        Vector3 direction = Accuracy(playerCameraTransform.forward, accuracy);
        RaycastHit hit;

        if (Physics.SphereCast(playerCameraTransform.position, sphereCastRadius, direction, out hit, effectiveRange) && hit.collider.tag == "Enemy"
            && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<EnemyController>() != null 
            && Vector3.Distance(playerCameraTransform.position, hit.point) <= minimumSpreadRange)
        {
            Debug.Log(hit.collider.gameObject.name);
            DamageEnemy(hit.collider.gameObject.GetComponent<EnemyController>(), pelletsPerShot);
        }
        else
        {
            for (int i = 0; i < pelletsPerShot; i++)
            {
                Vector3 shotDirection = Accuracy(direction, spreadAngle);
                RaycastHit raycastHit;

                if (Physics.Raycast(playerCameraTransform.position, shotDirection, out raycastHit, effectiveRange) && raycastHit.collider.tag == "Enemy"
                && raycastHit.collider.gameObject != null && raycastHit.collider.gameObject.GetComponent<EnemyController>() != null)
                {
                    GameObject marker = Instantiate(testPositionMarker, raycastHit.point, playerCameraTransform.rotation);
                    DamageEnemy(raycastHit.collider.gameObject.GetComponent<EnemyController>(), 1);

                    Destroy(marker, 2);
                }
            }
        }
    }

    public virtual void DamageEnemy(EnemyController enemyController, float pelletsThatHit)
    {
        enemyController.Damage((damage + ourPlayer.playerStats.stat[StatType.damage]) * pelletsThatHit);
    }
}
