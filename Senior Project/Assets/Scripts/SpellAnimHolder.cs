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

    public void CheckToRelease()
    {
        if (ourPuppet.currentSpellBeingCast == null || ourPuppet.currentSpellBeingCast.ourSpellState == SpellState.releasing)
        {
            return;
        }

        if (ourPuppet.currentSpellBeingCast == ourPuppet.primarySpell)
        {
            if (PlayerController.instance.onPrimaryFire.ReadValue<float>() <= 0.125)
            {
                if (ourPuppet.primarySpell.chargingSpell)
                {
                    PlayerController.instance.primaryFireHeldDown = false;
                    ReleaseSpell();
                }
            }
        }
        else if (ourPuppet.currentSpellBeingCast == ourPuppet.secondarySpell)
        {
            if (PlayerController.instance.onSecondaryFire.ReadValue<float>() <= 0.125)
            {
                if (ourPuppet.secondarySpell.chargingSpell)
                {
                    PlayerController.instance.secondaryFireHeldDown = false;
                    ReleaseSpell();
                }
            }
        }
    }
}
