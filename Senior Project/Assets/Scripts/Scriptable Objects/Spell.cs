using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum SpellSlot
{
    primary,
    secondary,
    utility
}

public enum SpellState
{
    charging,
    casting,
    releasing
}

public enum DamageType
{
    fire,
    ice,
    nuetral
}

public abstract class Spell : ScriptableObject
{

    [HideInInspector] public Transform playerCameraTransform; // The transform for the player camera
    public bool canCast = true; // A bool to decide if the gun can fire or not
    [HideInInspector] public SpellState ourSpellState;

    [Header("Spells Variables")]
    public SpellSlot ourSpellSlot;

    [Header("Animation Variables")]
    public Animator spellAnim; // An animator on the player hands
    [HideInInspector] public SpellAnimHolder spellAnimHolder;
    public int animationLocation = -1;
    [Tooltip("1 for the left hand, 2 for right, 0 for neither")]
    public int ourhand;

    [Header("Continuous spell")]
    [Tooltip("Check true if the spell needs to be held down/charged up before releasing.")]
    public bool chargingSpell;

    [Header("Temperature")]
    [Tooltip("Check true if the spell applies its temperature over time rather than in a burst.")]
    [ToggleableVarable("chargingSpell", true)] public bool tempPerSecond;
    public float temperatureChange;

    [Header("Attack Spells Variables")]
    public DamageType damageType;
    public float damage;

    [Tooltip("Check true if the spell uses raycasts rather than projectiles.")]
    public float effectiveRange;
    public float sphereCastRadius;
    [ToggleableVarable("chargingSpell", true)] public float timeBetweenProjectiles;
    [HideInInspector] public float timeBetweenProjectilesRemaining;

    [Header("Projectile and VFX")]
    public GameObject objectToSpawn;
    public GameObject vfxEffectObj;
    [HideInInspector] public VisualEffect vfx;

    [Header("Charges (Aka Ammo)")]
    public bool usesCharges;
    [ToggleableVarable("usesCharges", true)] public int charges;
    [ToggleableVarable("usesCharges", true)] public int maximumCharges;
    [ToggleableVarable("usesCharges", true)] public float rechargeRate;
    public float rechargeTimer;

    public virtual void InitializeSpell()
    {
        //Getting the variables for the player itself
        playerCameraTransform = PlayerController.puppet.cameraObj.transform;

        //After gets the variables on the player, it will spawn the in game model and gather the animator if available
        if (PlayerController.puppet.spellAnimObj != null)
        {
            // Getting the animator
            if (PlayerController.puppet.spellAnim != null)
            {
                spellAnim = PlayerController.puppet.spellAnim;
                spellAnimHolder = PlayerController.puppet.ourAnimHolder;
            }
        }

        if (vfxEffectObj != null)
        {
            Transform handTransform = GetFirePos();
            
            vfxEffectObj = Instantiate(vfxEffectObj, handTransform.position, handTransform.rotation);

            vfxEffectObj.transform.parent = handTransform;

            if (vfxEffectObj.GetComponent<VisualEffect>() != null)
            {
                vfx = vfxEffectObj.GetComponent<VisualEffect>();
            }
        }
    }

    public virtual void SpellUpdate()
    {
        if (chargingSpell)
        {
            switch (ourSpellState)
            {
                case (SpellState.charging):
                    Charging();
                    break;

                case (SpellState.casting):
                    Fire();
                    break;

                case (SpellState.releasing):
                    Releasing();
                    break;

                default:
                    break;
            }
        }
    }

    public virtual void SecondarySpellUpdate(float timeHolder)
    {
        if (usesCharges && charges < maximumCharges)
        {
            if (rechargeTimer <= 0)
            {
                charges++;
                if (charges < maximumCharges)
                {
                    rechargeTimer = rechargeRate;
                }
            }
            else
            {
                rechargeTimer -= timeHolder;
            }
        }
    }

    public virtual void Charging()
    {

    }

    public bool CastSpell()
    {
        if (usesCharges)
        {
            if (charges <= 0)
            {
                return false;
            }

            if (rechargeTimer <= 0)
            {
                rechargeTimer = rechargeRate;
            }
            
            if (chargingSpell)
            {
                Cast();
                return true;
            }
            else
            {
                Cast();
            }
        } 
        else if (chargingSpell)
        {
            Cast();
            return true;
        }
        else
        {
            Cast();
        }
        return false;
    }

    public virtual void Cast()
    {
        PlayerController.puppet.currentSpellBeingCast = this;

        // Checks if the gun can be fired and if there is ammo in the magazine
        if (canCast)
        {
            canCast = false;
            spellAnimHolder.DisableCasting();

            Debug.Log("Casted");

            if (usesCharges)
            {
                charges--;
            }

            if (animationLocation > 0 && spellAnimHolder != null)
            {
                // Debug.Log("FIRING");
                spellAnimHolder.SetAnimState(animationLocation);
            }
            else
            {
                ChangePlayerTemp();
                HitScanFire();
            }
        }
    }

    public virtual void Releasing()
    {
        
    }

    public virtual void Fire()
    {
        HitScanFire();
        ChangePlayerTemp();
    }

    public virtual void ChangePlayerTemp()
    {
        if (tempPerSecond)
        {
            PlayerController.puppet.ChangeTemperatureOfSelf(temperatureChange * Time.deltaTime);
        }
        else
        {
            PlayerController.puppet.ChangeTemperatureOfSelf(temperatureChange);
        }
        
    }

    public virtual void DamageEnemy(EnemyController enemyController)
    {
        // This simply calls a function to apply damage to the enemy at base
        // enemyController.Damage(damage * Time.deltaTime, damageType);
    }

    // The default firing method, hitscan does not spawn a projectile and instead utilizes a sphereCastAll
    public virtual void HitScanFire()
    {
        Vector3 direction = playerCameraTransform.forward;
        RaycastHit hit;
        Vector3 pos = GetFirePos().position;

        if (Physics.SphereCast(pos, sphereCastRadius, direction, out hit, effectiveRange) && hit.collider.tag == "Enemy"
            && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<EnemyController>() != null)
        {
            Debug.Log(hit.collider.gameObject.name);
                
            float damageToAssign = AssignDamage();
            hit.collider.gameObject.GetComponent<EnemyController>().Damage(damageToAssign, hit.point, damageType);
        }
    }

    public virtual void ProjectileFire()
    {
        GameObject newProjectile = SpawnManager.instance.GetGameObject(objectToSpawn, SpawnType.projectile);
        
        Vector3 pos = GetFirePos().position;
        Vector3 targetDirection = (FindTargetLocation() - pos).normalized;
        
        newProjectile.transform.position = pos;
        newProjectile.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Debug.Log(targetDirection + " " + playerCameraTransform.forward + " " + newProjectile.transform.eulerAngles);

        ProjectileController newProjectileScript = newProjectile.GetComponent<ProjectileController>();

        newProjectileScript.damage = AssignDamage();
        
        newProjectileScript.damageType = damageType;
        newProjectileScript.hostileFaction = Faction.Enemy;
        newProjectileScript.LaunchProjectile();
    }

    public virtual float AssignDamage()
    {
        float damageToReturn;
        if (chargingSpell)
        {
            damageToReturn = damage * timeBetweenProjectiles;
        }
        else
        {
            damageToReturn = damage;
        }

        if (damageType == DamageType.fire)
        {
            damageToReturn *= (1 + (PlayerController.puppet.damageMultiplier * PlayerController.puppet.fireMultiplier));
        }
        else if (damageType == DamageType.ice)
        {
            damageToReturn *= (1 + (PlayerController.puppet.damageMultiplier * PlayerController.puppet.iceMultiplier));
        }

        Debug.Log(System.Math.Round(damageToReturn));

        return damageToReturn;
    }

    public virtual Transform GetFirePos()
    {
        if (ourhand == 2)
        {
            return PlayerController.puppet.primaryFirePosition;
        }
        else if (ourhand == 1)
        {
            return PlayerController.puppet.secondaryFirePosition;
        }
        else
        {
            return playerCameraTransform;
        }
    }

    public virtual Vector3 FindTargetLocation()
    {
        Vector3 targetPos = Vector3.zero;
        Vector3 direction = playerCameraTransform.forward;
        RaycastHit hit;
        Transform firePos = GetFirePos();

        if (Physics.Raycast(playerCameraTransform.position, direction, out hit, effectiveRange, -1, QueryTriggerInteraction.Ignore))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = firePos.position + (direction * effectiveRange);
        }

        return targetPos;
    }

    public virtual Vector3 Accuracy(Vector3 forwardDirection, float variance)
    {
        // This subtracts the player's accuracy from 100, 
        // then divides it by 1000 to make it less extreme of an angle
        variance = variance / 1000;

        // This creates a new direction from the forward direction, then adds a random amount to the x and y value
        Vector3 newDirection = forwardDirection;

        Vector2 spread = Random.insideUnitCircle;
        newDirection += new Vector3(0f, spread.y, spread.x) * variance; 

        return newDirection;
    }

    public virtual void PlayVFX()
    {
        if (vfx != null)
        {
            if (vfxEffectObj.transform.rotation != playerCameraTransform.rotation)
            {
                vfxEffectObj.transform.rotation = playerCameraTransform.rotation;
            }

            vfx.Play();
        }
    }

    public virtual void StopVFX()
    {
        if (vfx != null)
        {
            vfx.Stop();
        }
    }
}
