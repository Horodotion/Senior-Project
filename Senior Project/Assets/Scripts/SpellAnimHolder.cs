using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAnimHolder : MonoBehaviour
{
    public PlayerPuppet ourPuppet;
    public string spellStateAnim = "";
    public string canCastInAnim = "";
    public string releaseSpell = "";
    public string canReleaseSpell = "";
    public bool castingSpell;


    public virtual void SetAnimState(int newState)
    {
        ourPuppet.spellAnim.SetInteger(spellStateAnim, newState);
    }

    public bool CanCast()
    {
        if (ourPuppet.spellAnim.GetInteger(spellStateAnim) != 0)
        {
            return false;
        }

        return true;
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
        if (ourPuppet.currentSpellBeingCast != null)
        {
            ourPuppet.currentSpellBeingCast.canCast = false;
            if (ourPuppet.currentSpellBeingCast.vfx != null)
            {
                ourPuppet.currentSpellBeingCast.vfx.Stop();
            }
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
            if (ourPuppet.currentSpellBeingCast.vfx != null)
            {
                ourPuppet.currentSpellBeingCast.vfx.Stop();
            }

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
        if (ourPuppet.currentSpellBeingCast == null)
        {
            return;
        }
    
        if (ourPuppet.currentSpellBeingCast.ourSpellState != SpellState.charging)
        {
            ourPuppet.currentSpellBeingCast.ourSpellState = SpellState.charging;
            ourPuppet.spellAnim.SetBool(canReleaseSpell, false);
        }
    }

    public void ActivateChargedSpell()
    {
        if (ourPuppet.currentSpellBeingCast == null)
        {
            return;
        }

        if (ourPuppet.currentSpellBeingCast.ourSpellState != SpellState.casting)
        {
            ourPuppet.currentSpellBeingCast.ourSpellState = SpellState.casting;
            ourPuppet.currentSpellBeingCast.PlayVFX();
        }
    }

    public void AllowRelease()
    {
        ourPuppet.spellAnim.SetBool(canReleaseSpell, true);
    }

    public void ReleaseSpell()
    {
        if (ourPuppet.currentSpellBeingCast == null)
        {
            return;
        }

        if (ourPuppet.currentSpellBeingCast.ourSpellState != SpellState.releasing)
        {
            ourPuppet.currentSpellBeingCast.ourSpellState = SpellState.releasing;
            ourPuppet.spellAnim.SetBool(releaseSpell, true);
            ourPuppet.currentSpellBeingCast.StopVFX();
        }
    }
}
