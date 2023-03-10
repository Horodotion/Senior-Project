using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageInteraction
{
    immune,
    vulnerable,
    resistant,
    nuetral
}

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public IndividualStat health;
    [HideInInspector] public bool inInvincibilityFrames = false;
    [HideInInspector] public bool dead = false;

    [Header("Damage Variations")]
    public List<DamageType> damageImmunities;
    public List<DamageType> damageResistances;
    public List<DamageType> damageVulnerabilities;
    public float vulnerabilityMultiplier;
    public float resistanceMultiplier;

    [Header("Hitbox List")]
    public List<EnemyHitbox> enemyHitboxes;

    public virtual void OnEnable()
    {
        if (enemyHitboxes.Count == 0)
        {
            foreach(EnemyHitbox hitbox in GetComponentsInChildren<EnemyHitbox>())
            {
                enemyHitboxes.Add(hitbox);
                hitbox.enemy = this;

                hitbox.vulnerabilityMultiplier = vulnerabilityMultiplier;
                hitbox.resistanceMultiplier = resistanceMultiplier;
            }
        }
    }

    public virtual void Damage(float damageAmount, DamageType damageType = DamageType.nuetral)
    {
        if (damageImmunities.Contains(damageType))
        {
            return;
        }

        float damage = DamageCalculation(damageAmount, damageType);

        // StartCoroutine(InvincibilityFrames());

        health.AddToStat(-damage);
        if (health.stat <= health.minimum && !dead)
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

    public virtual float DamageCalculation(float damage, DamageType damageType)
    {
        if (damageVulnerabilities.Contains(damageType))
        {
            return damage * vulnerabilityMultiplier;
        }
        else if (damageResistances.Contains(damageType))
        {
            return damage * resistanceMultiplier;
        }

        return damage;
    }

    public IEnumerator InvincibilityFrames()
    {
        inInvincibilityFrames = true;

        yield return new WaitForFixedUpdate();

        inInvincibilityFrames = false;
    }

    public virtual void ChangeDamageInteraction(DamageType damageType, DamageInteraction interaction)
    {
        EnemyController.ChangeDamageLists(this, damageType, interaction);

        foreach(EnemyHitbox hitbox in enemyHitboxes)
        {
            EnemyController.ChangeDamageLists(hitbox, damageType, interaction);
        }
    }

    public static void ChangeDamageLists(EnemyController enemyController, DamageType damageType, DamageInteraction interaction)
    {
        if (enemyController.damageImmunities.Contains(damageType))
        {
            RemoveDamageType(enemyController.damageImmunities, damageType, interaction == DamageInteraction.immune);
        }
        else
        {
            AddDamageType(enemyController.damageImmunities, damageType, interaction == DamageInteraction.immune);
        }

        if (enemyController.damageResistances.Contains(damageType))
        {
            RemoveDamageType(enemyController.damageResistances, damageType, interaction == DamageInteraction.resistant);
        }
        else
        {
            AddDamageType(enemyController.damageResistances, damageType, interaction == DamageInteraction.resistant);
        }

        if (enemyController.damageVulnerabilities.Contains(damageType))
        {
            RemoveDamageType(enemyController.damageVulnerabilities, damageType, interaction == DamageInteraction.vulnerable);
        }
        else
        {
            AddDamageType(enemyController.damageVulnerabilities, damageType, interaction == DamageInteraction.vulnerable);
        }
    }

    public static void RemoveDamageType(List<DamageType> list, DamageType damageType, bool matchesDamageType = false)
    {
        if (!matchesDamageType)
        {
            list.Remove(damageType);
        }
    }

    public static void AddDamageType(List<DamageType> list, DamageType damageType, bool matchesDamageType = false)
    {
        if (matchesDamageType)
        {
            list.Add(damageType);
        }
    }
}