using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/Beam")]
public class BeamAttack : Spell
{
    public override void HitScanFire()
    {
        Vector3 direction = playerCameraTransform.forward;
        RaycastHit hit;

        if (Physics.SphereCast(GetFirePos().position, sphereCastRadius, direction, out hit, effectiveRange) && hit.collider.tag == "Enemy"
            && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<EnemyController>() != null)
        {
            Debug.Log(hit.collider.gameObject.name);
            DamageEnemy(hit.collider.gameObject.GetComponent<EnemyController>());

            // GameObject marker = Instantiate(testPositionMarker, hit.point, playerCameraTransform.rotation);
            // Destroy(marker, 1f); // Destroying the marker to not have an infinite amount on screen
        }
    }

}
