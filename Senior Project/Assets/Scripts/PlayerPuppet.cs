using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    grounded,
    inAir,
    sliding,
    dashing,
    knockback,
    other
}

// This class acts as the actual game object to run around in the scene
// For the inputs and data storage, look to the PlayerController script
public class PlayerPuppet : MonoBehaviour
{
    //These references are localized for easier reading and writing of the script
    // [HideInInspector] public PlayerController PlayerController.instance; // A local  reference to the playerController
    // [HideInInspector] public Stats playerStats; // A reference to the stats for easier reading
    [HideInInspector] public Vector3 lookRotation; // The current lookRotation on the puppet
    [HideInInspector] public CharacterController charController; // A reference to the CharacterController
    public GameObject cameraObj; // The object that the camera is held on
    
    
    // [HideInInspector] public List<GameObject> interactableObjectList; // A list of objects that can be interacted with
    // public GameObject interactableObject; // A reference to what object we can currently interact with
    [HideInInspector] public GameObject spellAnimObj;
    [HideInInspector] public Animator spellAnim;
    [HideInInspector] public SpellAnimHolder ourAnimHolder;

    public float fallingSpeed = 0f; // The speed at which the player is currently falling

    /*
        A good amount of variables for physics and controls
        mouse sensetivity is how fast the camera moves, while look angles restricts the y values of the camera
        Gavity is how much gravity effects the player, while jump speed affects how high the player can jump
        spring multiplier is a speed buff to the player while running
    */
    [Header("Camera Movement Values")]
    public float mouseSensetivity = 1.0f;
    public float lookAngles = 90f;

    [Header("Movement Values")]
    public float sprintMultiplier = 2f;
    public float jumpSpeed = 1;
    public float gravity = 1;
    public float inAirControlMultiplier;

    [HideInInspector] public int jumpsRemaining = 2, totalJumps = 2;
    [HideInInspector] public bool canJump = true, grounded;
    public MovementState movementState;
    [HideInInspector] public bool isSliding = false;

    [Header("Spells")]
    [HideInInspector] public bool spellsSetUp;
    public Spell primarySpell;
    public Spell secondarySpell;
    public Spell mobilitySpell;
    public Spell currentSpellBeingCast;

    [Header("Fire Positions")]
    public Transform primaryFirePosition;
    public Transform secondaryFirePosition;
    [HideInInspector] public Vector3 moveDirection, inputDirection, velocity;
    [HideInInspector] public RaycastHit slidingHit;


    void Awake()
    {
        // Upon awake, it checks if there is currently a puppet on the player controller
        if (PlayerController.puppet == null)
        {
            // if there isn't a puppet, it sets itself to be the puppet
            PlayerController.puppet = this;

            // Then it gathers any local references needs, currently only on the CharacterController
            charController = GetComponent<CharacterController>();
        }
        else if (PlayerController.puppet != this)
        {
            // If there is already a puppet, the gameObject destroys itself
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        // At start, it gathers the referneces of the playerController and the stats
        // PlayerController.instance = PlayerController.instance;
        // playerStats = PlayerController.instance.playerStats;

        if (GetComponentInChildren<Animator>() != null)
        {
            spellAnim = GetComponentInChildren<Animator>();
        }

        if (GetComponentInChildren<SpellAnimHolder>() != null)
        {
            ourAnimHolder = GetComponentInChildren<SpellAnimHolder>();
            ourAnimHolder.ourPuppet = this;
        }

        PlayerController.instance.SetUpAllSpells();
    }

    void FixedUpdate()
    {
        switch(PlayerController.ourPlayerState)
        {  
            case PlayerState.inGame:
                SecondarySpellUpdate();
                Movement();
                break;

            case PlayerState.casting:
                SpellUpdater();
                Movement();
                break;

            case PlayerState.dashing:
                SpellUpdater();
                break;

            default:
                SecondarySpellUpdate();
                //Calling the function for movement for easier reading of the FixedUpdate
                Movement();
                break;
        }
    }
    
    // Late Update is needed to have the camera's y axis function properly
    void LateUpdate()
    {
        if (!GeneralManager.isGameRunning)
        {
            return;
        }

        // This checks if the look axis has moved
        if (PlayerController.instance.lookAxis != Vector2.zero)
        {
            // Adding to the look rotation multiplied by the mouse sensetivity and by time
            lookRotation.y += PlayerController.instance.lookAxis.x * mouseSensetivity * Time.fixedDeltaTime;
            // Makes sure that the player cannot infinitely spin up and down
            lookRotation.x = Mathf.Clamp((lookRotation.x - ((PlayerController.instance.lookAxis.y * mouseSensetivity) * Time.fixedDeltaTime)), -lookAngles, lookAngles);
            // Converts the added look rotation to the game object's rotation and camera object's rotation
            transform.localEulerAngles = new Vector3(0, lookRotation.y, 0);
            cameraObj.transform.localEulerAngles = new Vector3(lookRotation.x, 0, 0);
        }
    }


    //Functions that handle movement
    public void Movement()
    {
        grounded = charController.isGrounded;// || velocity.y == 0);
        isSliding = Sliding();
        inputDirection = HorizontalMovement();

        if (!grounded)//, charController.height))
        {
            Vector3 aerialVector = Vector3.zero;

            aerialVector = inputDirection * inAirControlMultiplier;

            float maxSpeed = PlayerController.instance.speed.stat * Time.deltaTime;
            
            moveDirection += new Vector3(aerialVector.x, 0, aerialVector.z) * Time.deltaTime;

            // moveDirection = Vector3.ClampMagnitude(moveDirection, maxSpeed);
            moveDirection.x = Mathf.Clamp(moveDirection.x, -maxSpeed, maxSpeed);
            moveDirection.z = Mathf.Clamp(moveDirection.z, -maxSpeed, maxSpeed);
            
            movementState = MovementState.inAir;
        }
        else
        {
            if (isSliding)
            {
                moveDirection += new Vector3(slidingHit.normal.x, 0, slidingHit.normal.z) * Time.deltaTime;
                movementState = MovementState.sliding;
            }
            else
            {
                moveDirection = inputDirection;
                movementState = MovementState.grounded;
            }
        }

        Falling();
        Jump();

        charController.Move(moveDirection);
        velocity = charController.velocity;
    }

    public Vector3 HorizontalMovement()
    {
        Vector3 vectorToReturn = Vector3.zero;
        // Checking if it's not empty
        if (PlayerController.instance.moveAxis != Vector2.zero)
        {
            // Changing the move axis to a Vector3
            vectorToReturn = new Vector3(PlayerController.instance.moveAxis.x, 0f, PlayerController.instance.moveAxis.y).normalized;

            // If the sprint key is held down, it multiplies the speed by the sprint multiplier
            // If not, it multiplies it by time and the player's speed stat

            vectorToReturn *= PlayerController.instance.speed.stat;

            if (PlayerController.instance.sprintHeldDown && vectorToReturn.z > 0)
            {
                vectorToReturn *= sprintMultiplier;
            }


            // After multiplying it by the speed, it will transform the direction of movement into world space
            vectorToReturn = transform.TransformDirection(vectorToReturn * Time.deltaTime);
        }
        return vectorToReturn;
    }

    public void AerialMovement()
    {
        

    }

    public void Falling()
    {
        // If the player is in the air, it will add one tenth of the gravity value to the falling speed

        moveDirection.y = 0;

        if (grounded && !isSliding)
        {
            if (jumpsRemaining != totalJumps)
            {
                jumpsRemaining = totalJumps;
            }
            
            if (fallingSpeed != -0.1f)
            {
                fallingSpeed = -0.1f;
            }
        }
        else
        {
            fallingSpeed -= (gravity / 10);// * Time.deltaTime;
            moveDirection.y += fallingSpeed * Time.deltaTime;
        }
    }

    public void Jump()
    {
        // If not, it checks if the player is trying to jump, then adds one tenth of the jump speed to the move
        if (PlayerController.instance.jumpHeldDown && (canJump && jumpsRemaining > 0))
        {
            canJump = false;
            jumpsRemaining--;
            fallingSpeed = jumpSpeed; //(JumpSpeed / 10);
            moveDirection.y = fallingSpeed * Time.deltaTime;
        }
        else if (!PlayerController.instance.jumpHeldDown)
        {
            canJump = true;
        }
    }

    public bool Sliding()
    {
        if (grounded && Physics.Raycast(transform.TransformPoint(charController.center), -transform.up, out slidingHit))
        {
            Debug.Log(slidingHit.collider.gameObject.name);

            return Vector3.Angle(slidingHit.normal, Vector3.up) > charController.slopeLimit;
        }
        else
        {
            return false;
        }
    }

    public void SpellUpdater()
    {
        if (currentSpellBeingCast != null)
        {
            currentSpellBeingCast.SpellUpdate();
        }
    }

    public void SecondarySpellUpdate()
    {
        if (primarySpell != null)
        {
            primarySpell.SecondarySpellUpdate();
        }

        if (secondarySpell != null)
        {
            secondarySpell.SecondarySpellUpdate();
        }

        if (mobilitySpell != null)
        {
            mobilitySpell.SecondarySpellUpdate();
        }
    }

    // A function to take damage from, currently only has a debug log
    public void Damage(float damageTaken)
    {
        // Debug.Log(damageTaken);
    }

    public void ChangeTemperature(float tempToAdd)
    {
        Debug.Log(tempToAdd);
        PlayerController.instance.temperature.AddToStat(tempToAdd);
        PlayerUI.instance.ChangeTemperature();
        // Debug.Log(playerStats.stat[StatType.temperature]);

        if (PlayerController.instance.temperature.stat >= PlayerController.instance.temperature.maximum ||
            PlayerController.instance.temperature.stat <= PlayerController.instance.temperature.minimum)
        {
            CommitDie();
        }
    }

    public void CommitDie()
    {
        // GeneralManager.ReturnToMainMenu();
        GeneralManager.instance.LoseGame();
    }
}