using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementState
{
    grounded,
    inAir,
    sliding,
    jumping,
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
    [HideInInspector] public CharacterController charController; // A reference to the CharacterController
    public GameObject cameraObj; // The object that the camera is held on
    
    // [HideInInspector] public List<GameObject> interactableObjectList; // A list of objects that can be interacted with
    // public GameObject interactableObject; // A reference to what object we can currently interact with
    public GameObject spellAnimObj;
    public Animator spellAnim;
    public SpellAnimHolder ourAnimHolder;

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
    [HideInInspector] public Vector3 lookRotation; // The current lookRotation on the puppet

    [Header("Movement Values")]
    public float sprintMultiplier = 2f;
    public float jumpSpeed = 1;
    public float gravity = 1;
    public float inAirControlMultiplier;

    [HideInInspector] public int jumpsRemaining = 2, totalJumps = 2;
    [HideInInspector] public bool canJump = true, grounded;
    public MovementState movementState;
    [HideInInspector] public bool isSliding = false;

    [Header("Temperature Multipliers")]
    public float escalationMultiplier;
    public float deescalationMultiplier;
    public float damageMultiplier;
    [Range(0f, 1f)] public float tempThreshold;
    public float tempMultiplier;
    [HideInInspector] public float fireMultiplier;
    [HideInInspector] public float iceMultiplier;

    [Header("Spells")]
    public Spell primarySpell;
    public Spell secondarySpell;
    public Spell mobilitySpell;
    public Spell currentSpellBeingCast;
    [HideInInspector] public bool spellsSetUp;

    [Header("Fire Positions")]
    public float highlightRange;
    public float highlightRadius;
    public Transform primaryFirePosition;
    public Transform secondaryFirePosition;
    [HideInInspector] public Vector3 moveDirection, inputDirection;
    [HideInInspector] public RaycastHit groundedHit;
    [HideInInspector] public RaycastHit reticalHit;

    [Header("Skin Shaders")]
    public SkinTemperatureScript fireSkin;
    public SkinTemperatureScript iceSkin;

    [Header("Sounds")]
    public AudioSource jumpSource;


    void Awake()
    {
        // Upon awake, it checks if there is currently a puppet on the player controller
        if (PlayerController.puppet == null)
        {
            // if there isn't a puppet, it sets itself to be the puppet
            PlayerController.puppet = this;

            // Then it gathers any local references needs, currently only on the CharacterController
            charController = GetComponent<CharacterController>();
            
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



        PlayerController.instance.SetUpAllSpells();
    }

    void FixedUpdate()
    {
        reticalHit = FindTargetLocation(); 
        SecondarySpellUpdate();

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
        inputDirection = HorizontalMovement();
        movementState = CheckMoveState();

        switch (movementState)
        {
            case MovementState.grounded:
                moveDirection = transform.TransformDirection(inputDirection * PlayerController.instance.speed.stat * Time.deltaTime);
                break;

            case MovementState.sliding:
                moveDirection += new Vector3(groundedHit.normal.x, 0, groundedHit.normal.z) * Time.deltaTime;
                break;

            case MovementState.inAir:
                AerialMovement();
                break;

            case MovementState.other:
                break;
        }

        Falling();
        Jump();

        charController.Move(moveDirection);
        // velocity = charController.velocity;
    }

    public Vector3 HorizontalMovement()
    {
        Vector3 vectorToReturn = Vector3.zero;
        // Checking if it's not empty
        if (PlayerController.instance.moveAxis != Vector2.zero)
        {
            if (Mathf.Abs(PlayerController.instance.moveAxis.x) >= 0.125f)
            {
                vectorToReturn.x = PlayerController.instance.moveAxis.x;
            }

            if (Mathf.Abs(PlayerController.instance.moveAxis.y) >= 0.125f)
            {
                vectorToReturn.z = PlayerController.instance.moveAxis.y;
            }

            vectorToReturn = vectorToReturn.normalized;

            // if (PlayerController.instance.sprintHeldDown && vectorToReturn.z > 0)
            // {
            //     vectorToReturn *= sprintMultiplier;
            // }
        }
        
        return vectorToReturn;
    }

    public MovementState CheckMoveState()
    {
        float adjustedHeight = (charController.height * 0.51f);
        bool groundDetected = Physics.SphereCast(transform.TransformPoint(charController.center), charController.radius, -transform.up, 
            out groundedHit, adjustedHeight) && !groundedHit.collider.isTrigger;


        if (groundDetected && fallingSpeed <= 0)
        {
            // Debug.Log(groundedHit.collider.gameObject.name + " + " + groundedHit.distance);
            if (Vector3.Angle(groundedHit.normal, Vector3.up) > charController.slopeLimit)
            {
                return MovementState.sliding;
            }
            else
            {
                return MovementState.grounded;
            }
        }
        else
        {
            return MovementState.inAir;
        }
    }

    public void Falling()
    {
        // If the player is in the air, it will add one tenth of the gravity value to the falling speed

        moveDirection.y = 0;

        if (movementState == MovementState.grounded)
        {
            if (jumpsRemaining != totalJumps)
            {
                jumpsRemaining = totalJumps;
            }
            
            if (fallingSpeed != 0)
            {
                fallingSpeed = 0;
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
            if (jumpSource != null)
            {
                jumpSource.Play();
            }

            canJump = false;
            jumpsRemaining--;
            fallingSpeed = jumpSpeed; //(JumpSpeed / 10);
            moveDirection.y = fallingSpeed * Time.deltaTime;

            jumpSource.Play();
        }
        else if (!PlayerController.instance.jumpHeldDown)
        {
            canJump = true;
        }
    }

    public void AerialMovement()
    {
        Vector3 aerialVector = Vector3.zero;
        float maxSpeed = PlayerController.instance.speed.stat * Time.deltaTime;

        aerialVector = transform.TransformDirection(inputDirection * PlayerController.instance.speed.stat * Time.deltaTime) * inAirControlMultiplier;
        moveDirection += new Vector3(aerialVector.x, 0, aerialVector.z) * Time.deltaTime;

        Vector3 horizontalVector = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), maxSpeed);
    
        moveDirection.x = horizontalVector.x;
        moveDirection.z = horizontalVector.z;
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
            ReduceSpellTimer(primarySpell);
            primarySpell.SecondarySpellUpdate(Time.deltaTime);
        }

        if (secondarySpell != null)
        {
            ReduceSpellTimer(secondarySpell);
            secondarySpell.SecondarySpellUpdate(Time.deltaTime);
        }

        if (mobilitySpell != null)
        {
            ReduceSpellTimer(mobilitySpell);
            mobilitySpell.SecondarySpellUpdate(Time.deltaTime);
        }
    }

    public void ReduceSpellTimer(Spell spellToReduce)
    {
        if (spellToReduce.usesCharges && spellToReduce.charges < spellToReduce.maximumCharges && 
            spellToReduce.rechargeTimer > spellToReduce.rechargeRate)
        {
            spellToReduce.rechargeTimer -= Time.deltaTime;
        }
    }

    public virtual RaycastHit FindTargetLocation()
    {
        Vector3 targetPos = Vector3.zero;
        Vector3 direction = cameraObj.transform.forward;
        RaycastHit hit;

        if (Physics.SphereCast(cameraObj.transform.position, highlightRadius, direction, out hit, highlightRange, -1, QueryTriggerInteraction.Ignore))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = cameraObj.transform.position + (direction);
        }

        return hit;
    }

    // A function to take damage from, currently only has a debug log
    public void Damage(float damageTaken)
    {
        // Debug.Log(damageTaken);
    }

    public void ResetStats()
    {
        PlayerController.instance.temperature.ResetStat();
        
        fireMultiplier = 0;
        iceMultiplier = 0;
        tempMultiplier = 0;

        ChangeTemperature(0);
    }

    public void ChangeTemperatureOfSelf(float tempToAdd)
    {
        float tempTemperatureToAdd = tempToAdd;


        if (tempMultiplier != 0)
        {
            if (Mathf.Sign(tempToAdd) == Mathf.Sign(PlayerController.instance.temperature.stat))
            {
                tempTemperatureToAdd *= 1 + (tempMultiplier * escalationMultiplier);
            }
            else
            {
                tempTemperatureToAdd *= 1 + (tempMultiplier * deescalationMultiplier);
            }
        }

        if ((PlayerController.instance.temperature.stat + tempTemperatureToAdd) >= PlayerController.instance.temperature.maximum * 0.95f)
        {
            PlayerController.instance.temperature.stat = PlayerController.instance.temperature.maximum * 0.95f;
            ChangeTemperature(0f);
        }
        else if ((PlayerController.instance.temperature.stat + tempTemperatureToAdd) <= PlayerController.instance.temperature.minimum * 0.95f)
        {
            PlayerController.instance.temperature.stat = PlayerController.instance.temperature.minimum * 0.95f;
            ChangeTemperature(0f);
        }
        else
        {
            ChangeTemperature(tempToAdd);
        }

    }

    public void ChangeTemperature(float tempToAdd)
    {
        IndividualStat temp = PlayerController.instance.temperature;

        if (tempMultiplier != 0)
        {
            if (Mathf.Sign(tempToAdd) == Mathf.Sign(temp.stat))
            {
                tempToAdd *= 1 + (tempMultiplier * escalationMultiplier);
            }
            else
            {
                tempToAdd *= 1 + (tempMultiplier * deescalationMultiplier);
            }
        }

        PlayerController.instance.temperature.AddToStat(tempToAdd);
        if (PlayerController.instance.temperature.stat >= PlayerController.instance.temperature.maximum ||
            PlayerController.instance.temperature.stat <= PlayerController.instance.temperature.minimum)
        {
            CommitDie();
            return;
        }

        fireSkin.SetShaderIntensity(Mathf.Clamp((temp.stat / temp.maximum), 0, 1));
        iceSkin.SetShaderIntensity(Mathf.Clamp((temp.stat / temp.minimum), 0, 1));

        if (temp.stat >= temp.maximum * tempThreshold)
        {
            fireMultiplier = (temp.stat - (temp.maximum * tempThreshold)) / (temp.maximum * (1 -  tempThreshold));
            tempMultiplier = fireMultiplier;
            iceMultiplier = 0;
        }
        else if (temp.stat <= temp.minimum * tempThreshold)
        {
            iceMultiplier = (temp.stat - (temp.minimum * tempThreshold)) / (temp.minimum * (1 - tempThreshold));
            tempMultiplier = iceMultiplier;
            fireMultiplier = 0;
        }
        else if (fireMultiplier != 0 || iceMultiplier != 0)
        {
            fireMultiplier = 0;
            iceMultiplier = 0;
            tempMultiplier = 0;
        }

        PlayerUI.instance.ChangeTemperature();
    }

    public void CommitDie()
    {
        // GeneralManager.ReturnToMainMenu();
        GeneralManager.instance.LoseGame();
    }
}