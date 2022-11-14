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
        PlayerController.ourPlayerState = PlayerState.dashing;
        dashOrigin = puppet.transform.position;

        if (PlayerController.instance.moveAxis != Vector2.zero)
        {
            dashDirection = new Vector3(ourPlayer.moveAxis.x, 0f, ourPlayer.moveAxis.y).normalized;
        }
        else
        {
            dashDirection = Vector3.forward;
        }

        dashDirection = puppet.transform.TransformDirection(dashDirection);
    }

    public override void SpellUpdate()
    {
        Vector3 dash = Time.deltaTime * dashSpeed * dashDirection;
        puppet.charController.Move(dash);
        
        float bigD = Vector3.Distance(puppet.transform.position, dashOrigin);

        if (bigD >= dashDistance)
        {
            Debug.Log(bigD);
            PlayerController.ourPlayerState = PlayerState.inGame;
            puppet.currentSpellBeingCast = null;
        }
    }
}
