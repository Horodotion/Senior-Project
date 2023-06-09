using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/Dash")]
public class Dash : Spell
{
    [Header("Dash")]

    public Vector3 dashDirection;
    [HideInInspector] public Vector3 dashOrigin;
    public float dashSpeed;
    public float dashDistance;

    public override void Cast()
    {
        PlayerController.puppet.currentSpellBeingCast = this;
        PlayerController.ourPlayerState = PlayerState.dashing;
        dashOrigin = PlayerController.puppet.transform.position;

        if (usesCharges)
        {
            charges--;
        }


        if (PlayerController.instance.moveAxis != Vector2.zero)
        {
            dashDirection = new Vector3(PlayerController.instance.moveAxis.x, 0f, PlayerController.instance.moveAxis.y).normalized;
        }
        else
        {
            dashDirection = Vector3.forward;
        }

        dashDirection = PlayerController.puppet.transform.TransformDirection(dashDirection);
    }

    public override void SpellUpdate()
    {
        Vector3 dash = Time.deltaTime * dashSpeed * dashDirection;
        PlayerController.puppet.charController.Move(dash);
        
        float bigD = Vector3.Distance(PlayerController.puppet.transform.position, dashOrigin);

        if (bigD >= dashDistance || PlayerController.puppet.charController.velocity == Vector3.zero)
        {
            Debug.Log(bigD);
            PlayerController.ourPlayerState = PlayerState.inGame;
            PlayerController.puppet.currentSpellBeingCast = null;
            if (vfx != null)
            {
                vfx.Stop();
            }
        }
    }

    public override void SecondarySpellUpdate(float timeHolder)
    {
        base.SecondarySpellUpdate(timeHolder);

        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.ChangeDashCounter(this);
        }
    }
}
