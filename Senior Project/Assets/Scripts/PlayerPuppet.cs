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
    [HideInInspector] public PlayerController ourPlayer; // A local  reference to the playerController
    [HideInInspector] public Stats playerStats; // A reference to the stats for easier reading
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
    public float mouseSensetivity = 1.0f, lookAngles = 90f, gravity = 1, jumpSpeed = 1, sprintMultiplier = 2f,
                 inAirControlMultiplier;
    [HideInInspector] public int jumpsRemaining = 2, totalJumps = 2;
    [HideInInspector] public bool canJump = true, grounded;
    public MovementState movementState;
    public bool isSliding = false;

    public Spell primarySpell, secondarySpell, mobilitySpell, currentSpellBeingCast;
    public Transform primaryFirePosition, secondaryFirePosition;
    public Vector3 moveDirection, inputDirection, velocity;
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
        ourPlayer = PlayerController.instance;
        playerStats = PlayerController.instance.playerStats;

        if (GetComponentInChildren<Animator>() != null)
        {
            spellAnim = GetComponentInChildren<Animator>();
        }

        if (GetComponentInChildren<SpellAnimHolder>() != null)
        {
            ourAnimHolder = GetComponentInChildren<SpellAnimHolder>();
            ourAnimHolder.ourPuppet = this;
        }
    }

    void FixedUpdate()
    {
        switch(PlayerController.ourPlayerState)
        {  
            case PlayerState.inGame:
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
                //Calling the function for movement for easier reading of the FixedUpdate
                Movement();
                break;
        }
    }
    
    // Late Update is needed to have the camera's y axis function properly
    void LateUpdate()
    {
        // This checks if the look axis has moved
        if (ourPlayer.lookAxis != Vector2.zero)
        {
            // Adding to the look rotation multiplied by the mouse sensetivity and by time
            lookRotation.y += ourPlayer.lookAxis.x * mouseSensetivity * Time.deltaTime;
            // Makes sure that the player cannot infinitely spin up and down
            lookRotation.x = Mathf.Clamp((lookRotation.x - ((ourPlayer.lookAxis.y * mouseSensetivity) * Time.deltaTime)), -lookAngles, lookAngles);
            // Converts the added look rotation to the game object's rotation and camera object's rotation
            transform.localEulerAngles = new Vector3(0, lookRotation.y, 0);
            cameraObj.transform.localEulerAngles = new Vector3(lookRotation.x, 0, 0);
        }
    }

    public void Movement()
    {
        grounded = charController.isGrounded;// || velocity.y == 0);
        isSliding = Sliding();

        if (!grounded)//, charController.height))
        {
            moveDirection = HorizontalMovement() * inAirControlMultiplier;
            movementState = MovementState.inAir;
        }
        else
        {
            inputDirection = HorizontalMovement();
            if (isSliding)
            {
                moveDirection += new Vector3(slidingHit.normal.x, 0, slidingHit.normal.z) * Time.deltaTime;
                movementState = MovementState.sliding;
            }
            else
            {
                moveDirection = inputDirection;
                movementState = MovementState.sliding;
            }
        }

        Falling();
        Jump();

        charController.Move(moveDirection);
        velocity = charController.velocity;
    }

    // The function that handles movement itself
    public Vector3 HorizontalMovement()
    {
        Vector3 vectorToReturn = Vector3.zero;
        // Checking if it's not empty
        if (ourPlayer.moveAxis != Vector2.zero)
        {
            // Changing the move axis to a Vector3
            vectorToReturn = new Vector3(ourPlayer.moveAxis.x, 0f, ourPlayer.moveAxis.y).normalized;

            // If the sprint key is held down, it multiplies the speed by the sprint multiplier
            // If not, it multiplies it by time and the player's speed stat
            if (PlayerController.instance.sprintHeldDown && vectorToReturn.z > 0)
            {
                vectorToReturn *= ourPlayer.playerStats.stat[StatType.speed] * sprintMultiplier;
            }
            else
            {
                vectorToReturn *= ourPlayer.playerStats.stat[StatType.speed];
            }

            // After multiplying it by the speed, it will transform the direction of movement into world space
            vectorToReturn = transform.TransformDirection(vectorToReturn * Time.deltaTime);
        }
        return vectorToReturn;
    }

    public void Falling()
    {
        // If the player is in the air, it will add one tenth of the gravity value to the falling speed

        if (grounded && !isSliding)
        {
            if (jumpsRemaining != totalJumps)
            {
                jumpsRemaining = totalJumps;
            }
            
            if (fallingSpeed != 0f)
            {
                fallingSpeed = 0f;
                moveDirection.y = 0;
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
        if (ourPlayer.jumpHeldDown && (canJump && jumpsRemaining > 0))
        {
            canJump = false;
            jumpsRemaining--;
            fallingSpeed = jumpSpeed; //(JumpSpeed / 10);
            moveDirection.y = fallingSpeed * Time.deltaTime;
        }
        else if (!ourPlayer.jumpHeldDown)
        {
            canJump = true;
        }
    }

    public bool Sliding()
    {
        if (grounded && Physics.Raycast(transform.TransformPoint(charController.center), -transform.up, out slidingHit))
        {
            return Vector3.Angle(slidingHit.normal, Vector3.up) > charController.slopeLimit;
        }
        else
        {
            return false;
        }
    }


    // The function that the useObject 
    public void UseObject()
    {

    }

    public void SpellUpdater()
    {
        if (currentSpellBeingCast != null)
        {
            currentSpellBeingCast.SpellUpdate();
        }
    }

    // A function to take damage from, currently only has a debug log
    public void Damage(float damageTaken)
    {
        // Debug.Log(damageTaken);
    }

    public void ChangeTemperature(float tempToAdd)
    {
        playerStats.AddToStat(StatType.temperature, tempToAdd);
        PlayerUI.instance.ChangeTemperature();
        // Debug.Log(playerStats.stat[StatType.temperature]);
    }
}