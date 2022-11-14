using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAnimHolder : MonoBehaviour
{
    public PlayerPuppet ourPuppet;
    public string spellStateAnim = "";
    public string canCastInAnim = "";
    public string releaseSpell = "";

    public virtual void SetAnimState(int newState)
    {
        ourPuppet.spellAnim.SetInteger(spellStateAnim, newState);
    }

    public virtual void EnableCasting()
    {
        if (ourPuppet.currentSpellBeingCast)
        {
            ourPuppet.currentSpellBeingCast.canCast = true;
        }
        
        ourPuppet.spellAnim.SetBool(canCastInAnim, true);
        ourPuppet.spellAnim.SetBool(releaseSpell, false);
    }

    public virtual void DisableCasting()
    {
        if (ourPuppet.currentSpellBeingCast)
        {
            ourPuppet.currentSpellBeingCast.canCast = false;
        }
        ourPuppet.spellAnim.SetBool(canCastInAnim, false);
        ourPuppet.spellAnim.SetBool(releaseSpell, false);
    }

    public void GoToIdle()
    {
        SetAnimState(0);

        PlayerController.ourPlayerState = PlayerState.inGame;

        if (ourPuppet.currentSpellBeingCast != null)
        {
            ourPuppet.currentSpellBeingCast.canCast = true;
            ourPuppet.currentSpellBeingCast = null;
        }
    }

    public void SpellFire()
    {
        if (ourPuppet.currentSpellBeingCast != null)
        {
            ourPuppet.currentSpellBeingCast.Fire();
        }
    }

    public void ChargeSpell()
    {
        if (ourPuppet.currentSpellBeingCast != null && ourPuppet.currentSpellBeingCast.ourSpellState != SpellState.charging)
        {
            ourPuppet.currentSpellBeingCast.ourSpellState = SpellState.charging;
        }
    }

    public void ActivateChargedSpell()
    {
        if (ourPuppet.currentSpellBeingCast && ourPuppet.currentSpellBeingCast.ourSpellState != SpellState.casting)
        {
            ourPuppet.currentSpellBeingCast.ourSpellState = SpellState.casting;

            if (ourPuppet.currentSpellBeingCast.vfx != null)
            {
                ourPuppet.currentSpellBeingCast.vfx.Play();
            }
        }
    }

    public void ReleaseSpell()
    {
        if (ourPuppet.currentSpellBeingCast != null && ourPuppet.currentSpellBeingCast.ourSpellState != SpellState.releasing)
        {
            ourPuppet.currentSpellBeingCast.ourSpellState = SpellState.releasing;
            ourPuppet.spellAnim.SetBool(releaseSpell, true);
            if (ourPuppet.currentSpellBeingCast.vfx != null)
            {
                ourPuppet.currentSpellBeingCast.vfx.Stop();
            }
        }
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
