using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Weapon/Revolver")]
public class Revolver : Weapon
{
    [HideInInspector] public float rechargeTimer;
    public float rechargeRate;
    public float ammoExpendedPerShot;

    public override void Fire()
    {
        if (ammo >= 1 && canFire)
        {
            rechargeTimer = rechargeRate;
            ammo -= ammoExpendedPerShot;
            HitScanFire();

            if (gunAnim != null)
            {
                gunAnim.SetInteger(gunStateAnim, 3);
            }
        }
    }

    public override void GunUpdate(float timeValue)
    {
        if (ammo < maxAmmo)
        {
            ammo += timeValue * rechargeRate;
        }
    }
}
