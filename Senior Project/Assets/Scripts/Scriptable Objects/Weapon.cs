using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is to decide what slot on the player that the weapon or something related to the weapon is related to
public enum WeaponSlot
{
    primary,
    secondary,
    heavy
}

// The weapon script here is a base class, and is meant to be iterated upon to make new types of weapon
public abstract class Weapon : ScriptableObject
{
    [HideInInspector] public PlayerController ourPlayer; // The player Controller that this weapon is attached to
    [HideInInspector] public PlayerPuppet puppet; // The player object that the controller is attached to
    [HideInInspector] public Transform playerCameraTransform; // The transform for the player camera
    [HideInInspector] public bool canFire = true; // A bool to decide if the gun can fire or not
    public GameObject testPositionMarker; // This is a testing market to spawn on hit for testing purposes
    public GameObject inGameModel; // The in game model for the gun itself
    public GunAnimationHolder gunAnimScript; // A script meant purely for the animations to affect code
    public Animator gunAnim; // An animator on the gun model
    public WeaponSlot weaponSlot; // What slot the weapon fits into
    public bool singleFire = false; // A bool for whether or not the gun is single fire or if it can be held down

    // Strings connected to the Animator
    [HideInInspector] public string canFireInAnim = "CanFire";
    [HideInInspector] public string gunStateAnim = "GunState";
    [HideInInspector] public string canHolsterInAnim = "CanHolster";

    /*
        A large amount of floats for stats on the weapon
        damage and knockback are meant to be on hit effects, knockback is currently not used
        ammo acts as the bullets in the magazine, and maxAmmo will add bullets back to the magazine
        higher accuracies will reduce the spread of the gun
        effective range increases how far the bullets will effeect damage falloff and how far aim assist works
        aim assist will act as a factor for how much forgiveness there is on the player's shots
    */
    public float damage, ammo, ammoInReserve, magazineSize, maxAmmo, accuracy, effectiveRange, knockBack, aimAssist = 10f;
    [HideInInspector] public float sphereCastRadius = 0.7f; // This is a value to see how large of a spherecast to use when shooting

    //InitializeGun is used to set up the gun itself after being equipped for the first time
    public virtual void InitializeGun(PlayerController player, PlayerPuppet newPuppet)
    {
        //Getting the variables for the player itself
        ourPlayer = player;
        puppet = newPuppet;
        playerCameraTransform = puppet.cameraObj.transform;

        //After gets the variables on the player, it will spawn the in game model and gather the animator if available
        if (inGameModel != null)
        {
            // Spawning the in game model
            inGameModel = Instantiate(inGameModel, playerCameraTransform.position, playerCameraTransform.rotation, playerCameraTransform);

            // Getting the animator
            if (inGameModel.GetComponent<Animator>() != null)
            {
                gunAnim = inGameModel.GetComponent<Animator>();
            }
            if (inGameModel.GetComponent<GunAnimationHolder>() != null)
            {
                gunAnimScript = inGameModel.GetComponent<GunAnimationHolder>();
                gunAnimScript.ourGun = this;
            }
        }
    }

    //Gun Update is meant to allow the gun to be updated by a monobehavior
    public virtual void GunUpdate(float timeValue)
    {
        // If the gun can be autofired and the player is holding down the trigger, fire the gun
        if (!singleFire && ourPlayer.fireHeldDown)
        {
            Fire();
        }
    }

    // This is the function that can fire the gun itself
    public virtual void Fire()
    {
        // Checks if the gun can be fired and if there is ammo in the magazine
        if (ammo > 0 && canFire)
        {
            // If both are true, it reduces the ammo in the magazine and by default acts as a hitscan weapon
            ammo--;
            HitScanFire();

            if (gunAnim != null)
            {
                gunAnim.SetInteger(gunStateAnim, 3);
            }
        }
        else if (ammo <= 0)
        {
            Reload();
        }
    }

    // The default firing method, hitscan does not spawn a projectile and instead utilizes a sphereCastAll
    public virtual void HitScanFire()
    {
        // This creates a new direction based on the weapon's accuracy stat
        Vector3 direction = Accuracy(playerCameraTransform.forward, accuracy);

        // After creating a new direction from accuracy, it gathers all colliders from within that direction
        RaycastHit[] raycastHits = Physics.SphereCastAll(playerCameraTransform.position, sphereCastRadius, direction, effectiveRange);
        
        //A set of variables that will be used to decide what was hit
        float tempAngle = Mathf.Infinity; // The lowest angle of what was hit
        GameObject tempHitEnemy = null; // The gameObject hit by the player
        Vector3 markerPosition = new Vector3(0, 0, 0); // A position to be used to spawn hitmarks, if availables


        //This for loop decides what was hit
        foreach (RaycastHit hit in raycastHits)
        {
            // Variables to ensure that we did hit an enemy
            RaycastHit newHit; // Gathering a new raycast to gather the data
            Vector3 hitDirection = hit.point - playerCameraTransform.position; // Getting the direction to spawn the raycast in
            float newAngle = Vector3.Angle(direction, hitDirection); // Checking the angle of said direction to compare against aim assist

            // If we hit something marked as an enemy, we are within the bounds of aim assist, 
            // and if we have line of sight to the hit object, then we compare it to what we currently have as our target
            if (hit.collider.tag == "Enemy" && newAngle <= aimAssist && Physics.Raycast(playerCameraTransform.position, hitDirection, out newHit)
                && newHit.collider.gameObject == hit.collider.gameObject)
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
            GameObject marker = Instantiate(testPositionMarker, markerPosition, playerCameraTransform.rotation);
            Destroy(marker, 1f); // Destroying the marker to not have an infinite amount on screen
        }
    }

    // The function that damages the enemy
    public virtual void DamageEnemy(EnemyController enemyController)
    {
        // This simply calls a function to apply damage to the enemy at base
        enemyController.Damage(damage + ourPlayer.playerStats.stat[StatType.damage]);
    }

    // Adding bullets to the magazine from maxAmmo, yet to be implimented
    public virtual void Reload()
    {
        if (gunAnim != null)
        {
            gunAnim.SetInteger(gunStateAnim, 2);
        }
        else
        {
            AddBulletsToMagazine();
        }
    }

    public virtual void AddBulletsToMagazine()
    {
        float ammoToReduce = magazineSize - ammo;
        ammo = magazineSize;
        ammoInReserve -= ammoToReduce;
    }

    // A function that creates a new Vector3 based on a float that maxes out at 100
    public virtual Vector3 Accuracy(Vector3 forwardDirection, float variance)
    {
        // This subtracts the player's accuracy from 100, 
        // then divides it by 1000 to make it less extreme of an angle
        variance = (100 - variance) / 1000;

        // This creates a new direction from the forward direction, then adds a random amount to the x and y value
        Vector3 newDirection = forwardDirection;
        newDirection.x += Random.Range(-variance, variance);
        newDirection.y += Random.Range(-variance, variance);

        return newDirection;
    }

    // This function makes this weapon be the player's current weapon
    public virtual void SwapToWeapon()
    {
        ourPlayer.currentWeapon = this;
        puppet.currentWeapon = this;
        if (gunAnim != null)
        {
            gunAnim.SetInteger(gunStateAnim, 1);
        }
    }

    // This function makes this weapon not be the player's current weapon
    public virtual void SwapOffWeapon()
    {
        if (gunAnim != null)
        {
            gunAnim.SetInteger(gunStateAnim, 0);
        }
        ourPlayer.currentWeapon = null;
        puppet.currentWeapon = null;
    }

    public virtual void AddAmmo(float ammoToAdd)
    {
        ammoInReserve = Mathf.Clamp(ammoInReserve + ammoToAdd, 0, (maxAmmo + magazineSize) - ammo);
    }

    public virtual void EnableFiring()
    {
        canFire = true;
        gunAnim.SetBool(canFireInAnim, true);
    }

    public virtual void DisableFiring()
    {
        canFire = false;
        gunAnim.SetBool(canFireInAnim, false);
    }

    public virtual void EnableWeaponSwapping()
    {
        gunAnim.SetBool(canHolsterInAnim, true);
    }

    public virtual void DisableWeaponSwapping()
    {
        gunAnim.SetBool(canHolsterInAnim, false);
    }

    public void GoToHolster()
    {
        gunAnim.SetInteger(gunStateAnim, 0);
    }

    public void GoToIdle()
    {
        gunAnim.SetInteger(gunStateAnim, 1);
    }
}
