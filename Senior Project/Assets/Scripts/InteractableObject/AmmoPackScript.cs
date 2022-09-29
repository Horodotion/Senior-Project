using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPackScript : MonoBehaviour
{
    // Choose which weapon slot will receive the ammo! This determines what gun the ammo pack will reload! Make sure the slot matches which model it correlates to!
    public WeaponSlot weaponSlot;
    // The float that determines how much the ammo pack refills
    public float ammoAmount;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            switch (weaponSlot)
            {
                case WeaponSlot.primary:
                    if (PlayerController.instance.primaryWeapon != null)
                    {
                        AddAmmo();
                    }
                    break;

                case WeaponSlot.secondary:
                    if (PlayerController.instance.secondaryWeapon != null)
                    {
                        AddAmmo();
                    }
                    break;

                case WeaponSlot.heavy:
                    if (PlayerController.instance.heavyWeapon != null)
                    {
                        AddAmmo();
                    }
                    break;

                default:
                    break;
            }
            // TO-DO   We're going to need an ammunition indicator on the HUD, then we need to call it to update.

            //SELF DESTRUCT ACTIVATED
            Destroy(gameObject);

        }
    }

    public void AddAmmo()
    {
        PlayerController.puppet.AddAmmo(weaponSlot, ammoAmount);
    }
}
