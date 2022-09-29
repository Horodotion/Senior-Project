using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimationHolder : MonoBehaviour
{
    public Weapon ourGun;

    public void EnableFiring()
    {
        ourGun.EnableFiring();
    }

    public void DisableFiring()
    {
        ourGun.DisableFiring();
    }

    public void EnableWeaponSwapping()
    {
        ourGun.EnableWeaponSwapping();
    }

    public void DisableWeaponSwapping()
    {
        ourGun.DisableWeaponSwapping();
    }

    public void AddBulletsToMagazine()
    {
        ourGun.AddBulletsToMagazine();
    }

    public void GoToHolster()
    {
        ourGun.GoToHolster();
    }

    public void GoToIdle()
    {
        ourGun.GoToIdle();
    }
}
