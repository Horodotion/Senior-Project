using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    inGame,
    casting,
    dashing,
    inMenu,
    other
}

// This script acts as the inputs and data storage for the players, 
// This acts as an input collector and data storage to transfer between scenes and menus
public class PlayerController : MonoBehaviour
{
    // Static variables to let this script and the puppet to be easily accessed
    public static PlayerController instance;
    public static PlayerPuppet puppet;
    public static PlayerState ourPlayerState;

    [Header("Stats")]
    // This is to act as the stats itself, and will carry between scenes with this setup
    // public Stats playerStats;
    public IndividualStat speed;
    public IndividualStat temperature;
    
    // A lot of hidden variables for the inputs, all stored in a PlayerInput component on the gameObject
    // They're hidden to allow for a cleaner set up in the inspector
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction onMove, onLook, onRun, onPrimaryFire, onSecondaryFire, onUse, onJump, onLastWeapon, onReload, onDash,
                                            onPauseGame;

    [HideInInspector] public bool sprintHeldDown, jumpHeldDown, primaryFireHeldDown, secondaryFireHeldDown;


    // A dictionary to keep track of Keys
    // public Dictionary<KeyType, Key> keyRing = new Dictionary<KeyType, Key>(); 
    [HideInInspector] public Vector2 moveAxis; // A Vector 2 that holds movement values
    [HideInInspector] public Vector2 lookAxis; // A vector 2 that holds the delta of the mouse to look around

    [Header("Spells")]
    // public Spell fireBasic, iceBasic, fireHeavy, iceHeavy, dash, blink;
    public Spell currentPrimarySpell;
    public Spell currentSecondarySpell;
    public Spell currentMobilitySpell;

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
            DontDestroyOnLoad(gameObject);

            // This sets the player's stats to a new copy to not override the original
            // and then initializes the stats
            // playerStats = Instantiate(playerStats);
            // playerStats.SetStats();

            // Cursor.lockState = CursorLockMode.Locked; // This sets the mouse to be locked into the center
            playerInput = GetComponent<PlayerInput>(); // This is getting the reference to the playerInput
            
            // These are simply gathering the references to all of the Input Actions to control the player
            onMove = playerInput.currentActionMap.FindAction("MoveAxis");
            onLook = playerInput.currentActionMap.FindAction("LookAxis");
            onRun = playerInput.currentActionMap.FindAction("Run");
            onPrimaryFire = playerInput.currentActionMap.FindAction("PrimaryFire");
            onSecondaryFire = playerInput.currentActionMap.FindAction("SecondaryFire");
            onUse = playerInput.currentActionMap.FindAction("Use");
            onJump = playerInput.currentActionMap.FindAction("Jump");
            onDash = playerInput.currentActionMap.FindAction("Dash");
            onPauseGame = playerInput.currentActionMap.FindAction("PauseGame");

            // These however, do have held down functions, and therefore need 2 functions to turn on and off
            onRun.started += OnRunAction;
            onRun.canceled += OffRunAction;
            onPrimaryFire.started += OnPrimaryFireAction;
            onPrimaryFire.canceled += OffPrimaryFireAction;
            onSecondaryFire.started += OnSecondaryFireAction;
            onSecondaryFire.canceled += OffSecondaryFireAction;
            onJump.started += OnJumpAction;
            onJump.canceled += OffJumpAction;
            onDash.performed += OnDashAction;
            onPauseGame.performed += OnPauseMenu;

            // This adds one of each type of key to the dictionary tracking them to allow easier coding later on
            // foreach (KeyType keyType in System.Enum.GetValues(typeof(KeyType)))
            // {
            //     keyRing.Add(keyType, null);
            // }
        }
    }

    public void Start()
    {
        if (puppet != null)
        {
            SetUpAllSpells();
        }
    }

    void Update()
    {
        //During update, it gathers the vectors for movement and looking from the inputActions
        moveAxis = onMove.ReadValue<Vector2>().normalized;
        lookAxis = onLook.ReadValue<Vector2>();
    }

    public void SetUpAllSpells()
    {
        SpellSetup(currentPrimarySpell, SpellSlot.primary);
        SpellSetup(currentSecondarySpell, SpellSlot.secondary);
        SpellSetup(currentMobilitySpell, SpellSlot.utility);
    }

    public Spell SpellSetup(Spell spellToSetUp, SpellSlot slot)
    {
        if (spellToSetUp != null)
        {
            spellToSetUp = Instantiate(spellToSetUp);

            switch (slot)
            {
                case SpellSlot.primary:
                    puppet.primarySpell = spellToSetUp;
                    break;

                case SpellSlot.secondary:
                    puppet.secondarySpell = spellToSetUp;
                    break;

                case SpellSlot.utility:
                    puppet.mobilitySpell = spellToSetUp;
                    break;

                default:
                    break;
            }

            spellToSetUp.InitializeSpell();

            return spellToSetUp;
        }
        else
        {
            Debug.Log("No Spell to set up");
            return null;
        }
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
    public void OnPrimaryFireAction(InputAction.CallbackContext context)
    {
        if (puppet == null)
        {
            return;
        }

        if (puppet.primarySpell != null && puppet.currentSpellBeingCast == null && ourPlayerState == PlayerState.inGame)
        {
            if (puppet.primarySpell.chargingSpell)
            {
                primaryFireHeldDown = true;
                ourPlayerState = PlayerState.casting;
                puppet.primarySpell.Cast();
                puppet.currentSpellBeingCast = puppet.primarySpell;
            }
            else
            {
                puppet.primarySpell.Cast();
                puppet.currentSpellBeingCast = puppet.primarySpell;
            }
        }
    }

    // A function for releasing the trigger
    public void OffPrimaryFireAction(InputAction.CallbackContext context)
    {
        if (puppet == null)
        {
            return;
        }

       if (puppet.primarySpell != null)
        {
            if (puppet.primarySpell.chargingSpell)
            {
                primaryFireHeldDown = false;
                puppet.ourAnimHolder.ReleaseSpell();
            }
            else
            {
                
            }
        }
    }

    // The function to call for the gun to shoot
    public void OnSecondaryFireAction(InputAction.CallbackContext context)
    {
        if (puppet == null)
        {
            return;
        }

        if (puppet.secondarySpell != null && puppet.currentSpellBeingCast == null && ourPlayerState == PlayerState.inGame)
        {
            if (puppet.secondarySpell.chargingSpell)
            {
                secondaryFireHeldDown = true;
                ourPlayerState = PlayerState.casting;
                puppet.secondarySpell.Cast();
                puppet.currentSpellBeingCast = puppet.secondarySpell;
            }
            else
            {
                puppet.secondarySpell.Cast();
                puppet.currentSpellBeingCast = puppet.secondarySpell;
            }
        }

    }

    // A function for releasing the trigger
    public void OffSecondaryFireAction(InputAction.CallbackContext context)
    {
        if (puppet == null)
        {
            return;
        }

        if (puppet.secondarySpell != null)
        {
            if (puppet.secondarySpell.chargingSpell)
            {
                secondaryFireHeldDown = false;
                puppet.ourAnimHolder.ReleaseSpell();
            }
            else
            {

            }
        }
    }

    // The button to allow interactable objects to be used
    public void OnUseAction(InputAction.CallbackContext context)
    {
        // // This only works from the puppet's side, and sends a message to it
        // if (puppet != null)
        // {
        //     puppet.UseObject();
        // }
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

    public void OnDashAction(InputAction.CallbackContext context)
    {
        if (puppet.mobilitySpell != null && puppet.currentSpellBeingCast == null && ourPlayerState == PlayerState.inGame)
        {
            puppet.currentSpellBeingCast = puppet.mobilitySpell;
            puppet.mobilitySpell.Cast();
        }
    }

    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (!GeneralManager.hasGameStarted)
        {
            return;
        }

        if (GeneralManager.isGameRunning)
        {
            GeneralManager.instance.PauseGame();
        }
        else
        {
            GeneralManager.instance.UnPauseGame();
        }
    }
}
