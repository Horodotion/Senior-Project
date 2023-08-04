using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/ProjectileAttack")]
public class ProjectileAttack : Spell
{
    public override void SpellUpdate()
    {
        base.SpellUpdate();

        if (timeBetweenProjectilesRemaining > 0)
        {
            timeBetweenProjectilesRemaining -= Time.deltaTime;
        }
    }
}
