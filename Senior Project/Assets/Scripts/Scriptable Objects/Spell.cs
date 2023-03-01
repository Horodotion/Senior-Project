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
    [HideInInspector] public bool canCast = true; // A bool to decide if the gun can fire or not
    [HideInInspector] public SpellState ourSpellState;

    [Header("Spells Variables")]
    public SpellSlot ourSpellSlot;

    [Header("Animation Variables")]
    public Animator spellAnim; // An animator on the player hands
    [HideInInspector] public SpellAnimHolder spellAnimHolder;
    public int animationLocation = -1;
    [Tooltip("1 for the left hand, 2 for right, 0 for neither")]
    public int ourhand;

    [Header("Single shot, or continuous")]
    [Tooltip("Check true if the spell needs to be held down/charged up before releasing.")]
    public bool chargingSpell;

    [Header("Temperature")]
    [Tooltip("Check true if the spell applies its temperature over time rather than in a burst.")]
    [ToggleableVarable("chargingSpell", true)] public bool tempPerSecond;
    public float temperatureChange;

    [Header("Attack Spells Variables")]
    public DamageType damageType;
    public float damage;
    public float effectiveRange;
    public float sphereCastRadius;

    [Header("Projectile and VFX")]
    public GameObject objectToSpawn;
    public GameObject vfxEffectObj;
    [HideInInspector] public VisualEffect vfx;

    [Header("Charges (Aka Ammo)")]
    public bool usesCharges;
    [ToggleableVarable("usesCharges", true)] public int charges;
    [ToggleableVarable("usesCharges", true)] public int maximumCharges;
    [ToggleableVarable("usesCharges", true)] public float rechargeRate;
    [HideInInspector] public float rechargeTimer;

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
            vfx = vfxEffectObj.GetComponent<VisualEffect>();
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

    public virtual void SecondarySpellUpdate()
    {

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

            charges--;
            rechargeTimer = rechargeRate;

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
            PlayerController.puppet.ChangeTemperature(temperatureChange * Time.deltaTime);
        }
        else
        {
            PlayerController.puppet.ChangeTemperature(temperatureChange);
        }
        
    }

    public virtual void DamageEnemy(EnemyController enemyController)
    {
        // This simply calls a function to apply damage to the enemy at base
        enemyController.Damage(damage, damageType);
    }

    // The default firing method, hitscan does not spawn a projectile and instead utilizes a sphereCastAll
    public virtual void HitScanFire()
    {
        // This creates a new direction based on the weapon's accuracy stat
        Vector3 direction = playerCameraTransform.forward; //Accuracy(playerCameraTransform.forward, accuracy);

        // After creating a new direction from accuracy, it gathers all colliders from within that direction
        RaycastHit[] raycastHits = Physics.SphereCastAll(GetFirePos().position, sphereCastRadius, direction, effectiveRange);
        
        //A set of variables that will be used to decide what was hit
        float tempAngle = Mathf.Infinity; // The lowest angle of what was hit
        GameObject tempHitEnemy = null; // The gameObject hit by the player
        Vector3 markerPosition = new Vector3(0, 0, 0); // A position to be used to spawn hitmarks, if availables


        //This for loop decides what was hit
        foreach (RaycastHit hit in raycastHits)
        {
            // Variables to ensure that we did hit an enemy
            RaycastHit newHit = new RaycastHit(); // Gathering a new raycast to gather the data
            Vector3 hitDirection = hit.point - GetFirePos().position; // Getting the direction to spawn the raycast in
            float newAngle = Vector3.Angle(direction, hitDirection); // Checking the angle of said direction to compare against aim assist

            // If we hit something marked as an enemy, we are within the bounds of aim assist, 
            // and if we have line of sight to the hit object, then we compare it to what we currently have as our target
            if (hit.collider.tag == "Enemy" && newHit.collider != null &&
                newHit.collider.gameObject == hit.collider.gameObject) // && newAngle <= aimAssist && Physics.Raycast(playerCameraTransform.position, hitDirection, out newHit)
            {

                // if the hit gameobject is closer to center than all others checked or if none have
                // been checked so far, it becomes the new target
                if (tempHitEnemy != null && newAngle < tempAngle)
                {
                    //This sets the temporary variables declared earlier to be used after the for loop
                    markerPosition = hit.point; 
                    tempHitEnemy = hit.collider.gameObject;
                    tempAngle = newAngle;
                }
                else if (tempHitEnemy == null)
                {
                    //This sets the temporary variables declared earlier to be used after the for loop
                    markerPosition = hit.point;
                    tempHitEnemy = hit.collider.gameObject;
                    tempAngle = newAngle;
                }
            }
        }

        // If we have a target to hit, this part is where we damage the enemy
        if (tempHitEnemy != null && tempHitEnemy.GetComponent<EnemyController>() != null)
        {
            // This is where we call the function to damage the enemy
            DamageEnemy(tempHitEnemy.GetComponent<EnemyController>());

            // These are all currently for debugging and testing purposes
            Debug.Log(tempHitEnemy.name); // Noting the enemy's name in the log
            // Spawning a hit marker where we hit the enemy
            // GameObject marker = Instantiate(testPositionMarker, markerPosition, playerCameraTransform.rotation);
            // Destroy(marker, 1f); // Destroying the marker to not have an infinite amount on screen
        }
    }

    public virtual void ProjectileFire()
    {
        GameObject newProjectile = SpawnManager.instance.GetGameObject(objectToSpawn, SpawnType.projectile);

        newProjectile.transform.position = GetFirePos().position;
        newProjectile.transform.rotation = PlayerController.puppet.cameraObj.transform.rotation;

        ProjectileController newProjectileScript = newProjectile.GetComponent<ProjectileController>();

        newProjectileScript.damage = damage;
        newProjectileScript.damageType = damageType;
        newProjectileScript.hostileFaction = Faction.Enemy;
        newProjectileScript.LaunchProjectile();
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

    public virtual Vector3 Accuracy(Vector3 forwardDirection, float variance)
    {
        // This subtracts the player's accuracy from 100, 
        // then divides it by 1000 to make it less extreme of an angle
        variance = variance / 1000;

        // This creates a new direction from the forward direction, then adds a random amount to the x and y value
        Vector3 newDirection = forwardDirection;

        Vector2 spread = Random.insideUnitCircle;
        newDirection += new Vector3(spread.x, spread.y, 0f) * variance; 

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
}
