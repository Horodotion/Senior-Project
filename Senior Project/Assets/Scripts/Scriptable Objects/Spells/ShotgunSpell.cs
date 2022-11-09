using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/Shotgun")]
public class ShotgunSpell : Spell
{

    [Header("Shotgun Variables")]
    public float pelletsPerShot;
    public float minimumSpreadRange;
    public float spreadAngle;

    public override void HitScanFire()
    {
        Vector3 direction = playerCameraTransform.forward;
        Vector3 raycastOrigin = GetFirePos().TransformPoint(Vector3.zero); //.position;
        RaycastHit hit;

        if (Physics.SphereCast(raycastOrigin, sphereCastRadius, direction, out hit, effectiveRange) && hit.collider.tag == "Enemy"
            && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<EnemyController>() != null 
            && Vector3.Distance(raycastOrigin, hit.point) <= minimumSpreadRange)
        {
            Debug.Log(hit.collider.gameObject.name);
            DamageEnemy(hit.collider.gameObject.GetComponent<EnemyController>(), pelletsPerShot);
        }
        else
        {
            for (int i = 0; i < pelletsPerShot; i++)
            {
                Vector3 shotDirection = Accuracy(direction, spreadAngle);
                // RaycastHit raycastHit;

                // if (Physics.Raycast(raycastOrigin, shotDirection, out raycastHit, effectiveRange) && raycastHit.collider.tag == "Enemy"
                // && raycastHit.collider.gameObject != null && raycastHit.collider.gameObject.GetComponent<EnemyController>() != null)
                // {
                //     GameObject marker = Instantiate(testPositionMarker, raycastHit.point, playerCameraTransform.rotation);
                //     DamageEnemy(raycastHit.collider.gameObject.GetComponent<EnemyController>(), 1);

                //     Destroy(marker, 2);
                // }

                if (objectToSpawn != null)
                {
                    GameObject iceParticle = Instantiate(objectToSpawn, raycastOrigin, Quaternion.LookRotation(shotDirection, Vector3.up));

                }
            }
        }
    }

    public virtual void DamageEnemy(EnemyController enemyController, float pelletsThatHit)
    {
        enemyController.Damage((damage) * pelletsThatHit);
    }
}
