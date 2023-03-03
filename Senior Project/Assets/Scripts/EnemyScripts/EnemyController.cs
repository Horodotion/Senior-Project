using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    // public Stats stats;
    public IndividualStat health;
    [HideInInspector] public bool inInvicibilityFrames = false;
    [HideInInspector] public bool dead = false;
    public List<DamageType> damageImmunities;

    public virtual void Damage(float damageAmount, DamageType damageType)
    {
        if (damageImmunities.Contains(damageType))
        {
            return;
        }

        health.AddToStat(-damageAmount);
        if (health.stat <= health.minimum)
        {
            CommitDie();
        }
    }

    public virtual void CommitDie()
    {
        dead = true;
        Debug.Log(gameObject.name + " is dead");
    }

    public virtual void Explode()
    {
        Debug.Log("Boom");
    }
}
