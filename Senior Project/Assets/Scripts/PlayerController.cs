using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// This script acts as the inputs and data storage for the players, 
// This acts as an input collector and data storage to transfer between scenes and menus
public class PlayerController : MonoBehaviour
{
    // Static variables to let this script and the puppet to be easily accessed
    public static PlayerController instance;
    public static PlayerPuppet puppet;
    // This is to act as the stats itself, and will carry between scenes with this setup
    public Stats playerStats;
    
    // A lot of hidden variables for the inputs, all stored in a PlayerInput component on the gameObject
    // They're hidden to allow for a cleaner set up in the inspector
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction onMove; // Defaults to WASD
    [HideInInspector] public InputAction onLook; // Defaults to mouse movement
    [HideInInspector] public InputAction onRun; // Defaults to left shift
    [HideInInspector] public InputAction onFire; // Defaults to left click
    [HideInInspector] public InputAction onUse; // Defaults to E
    [HideInInspector] public InputAction onJump; // Defaults to the spacebar
    [HideInInspector] public InputAction onPrimaryWeapon; // Defaults to 1
    [HideInInspector] public InputAction onSecondaryWeapon; // Defaults to 2
    [HideInInspector] public InputAction onHeavyWeapon; // Defaults to 3
    [HideInInspector] public InputAction onLastWeapon; // Defaults to Q
    [HideInInspector] public InputAction onReload; // Defaults to R
    [HideInInspector] public bool sprintHeldDown; // A bool to keep track of whether shift is being held down
    [HideInInspector] public bool jumpHeldDown; // A bool to keep track of whether space is being held down
    [HideInInspector] public bool fireHeldDown; // A bool to keep track of whether space is being held down
    // A dictionary to keep track of Keys
    public Dictionary<KeyType, Key> keyRing = new Dictionary<KeyType, Key>(); 
    public Vector2 moveAxis; // A Vector 2 that holds movement values
    public Vector2 lookAxis; // A vector 2 that holds the delta of the mouse to look around

    public Weapon currentWeapon; // The player's currently equipped weapon
    public Weapon primaryWeapon; // The player's primary weapon
    public Weapon secondaryWeapon; // The player's secondary weapon
    public Weapon heavyWeapon; // The player's heavy weapon


    void Awake()
    {
        // The player controller is static, so this checks if there is already a playerController
        if (instance != null && instance != this)
        {
            // If there is already a playerController, this copy will destroy itself
            Destroy(this.gameObject);
        }
        else
        {
            // If there isn't a playerController, this wil become the new playerController
            instance = this;

            // This sets the player's stats to a new copy to not override the original
            // and then initializes the stats
            playerStats = Instantiate(playerStats);
            playerStats.SetStats();

            Cursor.lockState = CursorLockMode.Locked; // This sets the mouse to be locked into the center
            playerInput = GetComponent<PlayerInput>(); // This is getting the reference to the playerInput
            
            // These are simply gathering the references to all of the Input Actions to control the player
            onMove = playerInput.currentActionMap.FindAction("MoveAxis");
            onLook = playerInput.currentActionMap.FindAction("LookAxis");
            onRun = playerInput.currentActionMap.FindAction("Run");
            onFire = playerInput.currentActionMap.FindAction("Fire");
            onUse = playerInput.currentActionMap.FindAction("Use");
            onJump = playerInput.currentActionMap.FindAction("Jump");
            onPrimaryWeapon = playerInput.currentActionMap.FindAction("PrimaryWeapon");
            onSecondaryWeapon = playerInput.currentActionMap.FindAction("SecondaryWeapon");
            onHeavyWeapon = playerInput.currentActionMap.FindAction("HeavyWeapon");
            onReload = playerInput.currentActionMap.FindAction("Reload");

            // These are setting up the functions to be triggered by the Input Actions
            // These do not currently have a held down function, and only need one function
            onUse.performed += OnUseAction;
            onPrimaryWeapon.performed += OnPrimaryWeaponAction;
            onSecondaryWeapon.performed += OnSecondaryWeaponAction;
            onHeavyWeapon.performed += OnHeavyWeaponAction;
            onReload.performed += OnReloadWeaponAction;

            // These however, do have held down functions, and therefore need 2 functions to turn on and off
            onRun.started += OnRunAction;
            onRun.canceled += OffRunAction;
            onFire.started += OnFireAction;
            onFire.canceled += OffFireAction;
            onJump.started += OnJumpAction;
            onJump.canceled += OffJumpAction;

            // This adds one of each type of key to the dictionary tracking them to allow easier coding later on
            foreach (KeyType keyType in System.Enum.GetValues(typeof(KeyType)))
            {
                keyRing.Add(keyType, null);
            }

            // Currently start is only used to set up all of the weapons that begin equipped to the player
/*
            // This checks if there is a primary weapon
            if (primaryWeapon != null)
            {
                // If there is a primary weapon, then it spawns a copy of the serialized object and sets it up
                primaryWeapon = Instantiate(primaryWeapon);
                puppet.primaryWeapon = primaryWeapon;
                primaryWeapon.InitializeGun(this, puppet); // This sets the references of itself and the puppet

                // If there are no other weapons equipped, the primary weapon becomes the current weapon
                if (currentWeapon == null)
                {
                    primaryWeapon.SwapToWeapon();
                }
            }
            // This checks if there is a secondary weapon
            if (secondaryWeapon != null)
            {
                // If there is a secondary weapon, then it spawns a copy of the serialized object and sets it up
                secondaryWeapon = Instantiate(secondaryWeapon);
                puppet.secondaryWeapon = secondaryWeapon;
                secondaryWeapon.InitializeGun(this, puppet); // This sets the references of itself and the puppet

                // If there are no other weapons equipped, the secondary weapon becomes the current weapon
                if (currentWeapon == null)
                {
                    secondaryWeapon.SwapToWeapon();
                }
            }
            // This checks if there is a heavy weapon
            if (heavyWeapon != null)
            {
                // If there is a heavy weapon, then it spawns a copy of the serialized object and sets it up
                heavyWeapon = Instantiate(heavyWeapon);
                puppet.heavyWeapon = heavyWeapon;
                heavyWeapon.InitializeGun(this, puppet); // This sets the references of itself and the puppet

                // If there are no other weapons equipped, the heavy weapon becomes the current weapon
                if (currentWeapon == null)
                {
                    heavyWeapon.SwapToWeapon();
                }
            }
*/
        }
    }

    // void Start()
    // {
    //     // Currently start is only used to set up all of the weapons that begin equipped to the player

    //     // This checks if there is a primary weapon
    //     if (primaryWeapon != null)
    //     {
    //         // If there is a primary weapon, then it spawns a copy of the serialized object and sets it up
    //         primaryWeapon = Instantiate(primaryWeapon);
    //         puppet.primaryWeapon = primaryWeapon;
    //         primaryWeapon.InitializeGun(this, puppet); // This sets the references of itself and the puppet

    //         // If there are no other weapons equipped, the primary weapon becomes the current weapon
    //         if (currentWeapon == null)
    //         {
    //             primaryWeapon.SwapToWeapon();
    //         }
    //     }
    //     // This checks if there is a secondary weapon
    //     if (secondaryWeapon != null)
    //     {
    //         // If there is a secondary weapon, then it spawns a copy of the serialized object and sets it up
    //         secondaryWeapon = Instantiate(secondaryWeapon);
    //         puppet.secondaryWeapon = secondaryWeapon;
    //         secondaryWeapon.InitializeGun(this, puppet); // This sets the references of itself and the puppet

    //         // If there are no other weapons equipped, the secondary weapon becomes the current weapon
    //         if (currentWeapon == null)
    //         {
    //             secondaryWeapon.SwapToWeapon();
    //         }
    //     }
    //     // This checks if there is a heavy weapon
    //     if (heavyWeapon != null)
    //     {
    //         // If there is a heavy weapon, then it spawns a copy of the serialized object and sets it up
    //         heavyWeapon = Instantiate(heavyWeapon);
    //         puppet.heavyWeapon = heavyWeapon;
    //         heavyWeapon.InitializeGun(this, puppet); // This sets the references of itself and the puppet

    //         // If there are no other weapons equipped, the heavy weapon becomes the current weapon
    //         if (currentWeapon == null)
    //         {
    //             heavyWeapon.SwapToWeapon();
    //         }
    //     }
    // }

    void Update()
    {
        //During update, it gathers the vectors for movement and looking from the inputActions
        moveAxis = onMove.ReadValue<Vector2>().normalized;
        lookAxis = onLook.ReadValue<Vector2>();
    }

    // The function to allow sprinting to begin
    public void OnRunAction(InputAction.CallbackContext context)
    {
        sprintHeldDown = true;
    }

    // The function to allow sprinting to stop
    public void OffRunAction(InputAction.CallbackContext context)
    {
        sprintHeldDown = false;
    }

    // The function to call for the gun to shoot
    public void OnFireAction(InputAction.CallbackContext context)
    {
        // Checking if there is a current weapon
        if (currentWeapon != null)
        {
            // If the weapon is single fire, it simple calls for the gun to fire
            // If not, it counts the trigger as being held down
            if (currentWeapon.singleFire)
            {
                currentWeapon.Fire();
            }
            else
            {
                fireHeldDown = true;
            }
        }
    }

    // A function for releasing the trigger
    public void OffFireAction(InputAction.CallbackContext context)
    {
        // If the current weapon exists and isn't single fire, it pulls off of the trigger
        if (currentWeapon != null && !currentWeapon.singleFire)
        {
            fireHeldDown = false;
        }
    }

    // The button to allow interactable objects to be used
    public void OnUseAction(InputAction.CallbackContext context)
    {
        // This only works from the puppet's side, and sends a message to it
        if (puppet != null)
        {
            puppet.UseObject();
        }
    }

    // This calls for the player to start jumping
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        jumpHeldDown = true;
    }

    //This calls for the player to stop jumping
    public void OffJumpAction(InputAction.CallbackContext context)
    {
        jumpHeldDown = false;
    }

    // This function swaps to the primary weapon
    public void OnPrimaryWeaponAction(InputAction.CallbackContext context)
    {
        // If there is a primary weapon and it's not already equipped, it swaps to the primary weapon
        if (primaryWeapon != null && currentWeapon != primaryWeapon)
        {
            // A null check in case there isn't any weapon equipped, then calls for the current weapon to lower
            if (currentWeapon != null)
            {
                currentWeapon.SwapOffWeapon();
            }

            // This calls for the primary weapon to begin
            primaryWeapon.SwapToWeapon();
        }
    }

    // This function swaps to the secondary weapon
    public void OnSecondaryWeaponAction(InputAction.CallbackContext context)
    {
        // If there is a secondary weapon and it's not already equipped, it swaps to the secondary weapon
        if (secondaryWeapon != null && currentWeapon != secondaryWeapon)
        {
            // A null check in case there isn't any weapon equipped, then calls for the current weapon to lower
            if (currentWeapon != null)
            {
                currentWeapon.SwapOffWeapon();
            }

            // This calls for the secondary weapon to begin
            secondaryWeapon.SwapToWeapon();
        }
    }

    // This function swaps to the heavy weapon
    public void OnHeavyWeaponAction(InputAction.CallbackContext context)
    {
        // If there is a heavy weapon and it's not already equipped, it swaps to the heavy weapon
        if (heavyWeapon != null && currentWeapon != heavyWeapon)
        {
            // A null check in case there isn't any weapon equipped, then calls for the current weapon to lower
            if (currentWeapon != null)
            {
                currentWeapon.SwapOffWeapon();
            }

            // This calls for the heavy weapon to begin
            heavyWeapon.SwapToWeapon();
        }
    }

    public void OnReloadWeaponAction(InputAction.CallbackContext context)
    {
        // If there is a primary weapon and it's not already equipped, it swaps to the primary weapon
        if (currentWeapon != null && currentWeapon.ammo < currentWeapon.magazineSize)
        {
            currentWeapon.Reload();
        }
    }
}
