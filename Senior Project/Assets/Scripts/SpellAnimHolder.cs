using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAnimHolder : MonoBehaviour
{
    public PlayerPuppet ourPuppet;
    public string spellStateAnim = "";
    public string canCastInAnim = "";

    public virtual void SetAnimState(int newState)
    {
        ourPuppet.spellAnim.SetInteger(spellStateAnim, newState);
    }

    public virtual void EnableCasting()
    {
        ourPuppet.currentSpellBeingCast.canCast = true;
        ourPuppet.spellAnim.SetBool(canCastInAnim, true);
    }

    public virtual void DisableCasting()
    {
        ourPuppet.currentSpellBeingCast.canCast = false;
        ourPuppet.spellAnim.SetBool(canCastInAnim, false);
    }

    public void GoToIdle()
    {
        SetAnimState(0);
        ourPuppet.currentSpellBeingCast = null;
    }

    public void SpellFire()
    {
        ourPuppet.currentSpellBeingCast.Fire();
    }

    // public Weapon ourGun;

    // public void EnableFiring()
    // {
    //     ourGun.EnableFiring();
    // }

    // public void DisableFiring()
    // {
    //     ourGun.DisableFiring();
    // }

    // public void EnableWeaponSwapping()
    // {
    //     ourGun.EnableWeaponSwapping();
    // }

    // public void DisableWeaponSwapping()
    // {
    //     ourGun.DisableWeaponSwapping();
    // }

    // public void AddBulletsToMagazine()
    // {
    //     ourGun.AddBulletsToMagazine();
    // }

    // public void GoToHolster()
    // {
    //     ourGun.GoToHolster();
    // }

    // public void GoToIdle()
    // {
    //     ourGun.GoToIdle();
    // }
}
