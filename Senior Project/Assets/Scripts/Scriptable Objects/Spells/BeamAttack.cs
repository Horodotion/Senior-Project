using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/Beam")]
public class BeamAttack : Spell
{
    [HideInInspector] public Vector3 beamPos;

    public override void InitializeSpell()
    {
        base.InitializeSpell();
        
        if (vfxEffectObj != null)
        {
            vfxEffectObj.SetActive(false);

            if (vfxEffectObj.GetComponent<AudioSource>() != null)
            {
                vfxEffectObj.GetComponent<AudioSource>().Stop();
            }
        }
    }

    public override void HitScanFire()
    {
        Vector3 direction = playerCameraTransform.forward;
        RaycastHit hit;
        Transform firePos = GetFirePos();

        if (Physics.SphereCast(playerCameraTransform.position, sphereCastRadius, direction, out hit, effectiveRange) && hit.collider != null)
        {
            if (hit.collider.tag == "Enemy" && hit.collider.gameObject.GetComponent<EnemyController>() != null)
            {
                float damageToAssign = AssignDamage();
                hit.collider.gameObject.GetComponent<EnemyController>().Damage(damageToAssign * Time.deltaTime, hit.point, damageType);
            }

            beamPos = hit.point;
        }
        else
        {
            beamPos = firePos.position + (direction * effectiveRange);
        }

        if (vfxEffectObj != null)
        {
            vfxEffectObj.GetComponent<BeamScript>().ChangeEndPosition(beamPos);
        }
    }

    public override void PlayVFX()
    {
        if (vfxEffectObj != null)
        {
            if (vfxEffectObj.transform.rotation != playerCameraTransform.rotation)
            {
                vfxEffectObj.transform.rotation = playerCameraTransform.rotation;
            }

            vfxEffectObj.SetActive(true);

            if (vfxEffectObj.GetComponent<AudioSource>() != null)
            {
                vfxEffectObj.GetComponent<AudioSource>().Play();
            }
        }
    }

    public override void StopVFX()
    {
        if (vfxEffectObj != null)
        {
            vfxEffectObj.SetActive(false);

            if (vfxEffectObj.GetComponent<AudioSource>() != null)
            {
                vfxEffectObj.GetComponent<AudioSource>().Stop();
            }
        }
    }

}
