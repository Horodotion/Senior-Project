using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class acts as the actual game object to run around in the scene
// For the inputs and data storage, look to the PlayerController script
public class PlayerPuppet : MonoBehaviour
{
    //These references are localized for easier reading and writing of the script
    [HideInInspector] public PlayerController ourPlayer; // A local  reference to the playerController
    [HideInInspector] public Stats playerStats; // A reference to the stats for easier reading
    [HideInInspector] public Vector3 lookRotation; // The current lookRotation on the puppet
    [HideInInspector] public CharacterController charController; // A reference to the CharacterController
    [HideInInspector] public List<GameObject> interactableObjectList; // A list of objects that can be interacted with
    
    public GameObject cameraObj; // The object that the camera is held on
    public GameObject interactableObject; // A reference to what object we can currently interact with
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
    public float mouseSensetivity = 1.0f, lookAngles = 90f, Gravity = 1, JumpSpeed = 1, SprintMultiplier = 2f,
                 inAirControlMultiplier;
    [HideInInspector] public int jumpsRemaining = 2, totalJumps = 2;
    [HideInInspector] public bool canJump = true;

    public Spell primarySpell, secondarySpell, mobilitySpell, currentSpellBeingCast;
    public Transform primaryFirePosition, secondaryFirePosition;


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

    // OnTriggerEnter currently is used to gather the list of interactable Objects
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Interactable" && col.GetComponent<Interactable>())
        {
            // Setting up variables to check fo the closest interactable object
            RaycastHit hit;
            Vector3 checkingRaycastDirection = col.bounds.center - cameraObj.transform.position;

            //This checks if there is line of sight, and then whichever object is closest to the center
            if (Physics.Raycast(cameraObj.transform.position, checkingRaycastDirection, out hit) 
                    && hit.collider.gameObject == col.gameObject)
            {
                //If all of the above is true, it adds the gameObject to the list of possible interactable objects
                interactableObjectList.Add(col.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        //If it's on the list of interactable objects, this removes it from the list
        if (interactableObjectList.Contains(col.gameObject))
        {
            interactableObjectList.Remove(col.gameObject);
        }
    }

    // A function to return the closest interactable object to the player
    public GameObject GetClosestInteractableObject()
    {
        // If there's more than 1 gameObject in the list of interactable objects, it calculates
        if (interactableObjectList.Count > 1)
        {
            // A list of variables to check against
            GameObject tempClosestItem = null; // The game object to return
            float tempClosestDistance = Mathf.Infinity; // The closest distance to check against
            float tempDistance; // The distance to calculate against tempClosestDistance

            for (int i = 0; i < interactableObjectList.Count; i++)
            {
                // This calculates distance then checks if it is closer or if the object is null
                tempDistance = Vector3.Distance(cameraObj.transform.position, interactableObjectList[i].transform.position);
                if (tempDistance < tempClosestDistance || tempClosestItem == null)
                {
                    //if all of the above are true, it sets the tempClosest object and tempClosestDistance
                    tempClosestItem = interactableObjectList[i].gameObject;
                    tempClosestDistance = tempDistance;
                    Debug.Log(tempClosestItem.name + " Temp"); // A debug log to check that we did hit something
                }
            }
            
            return tempClosestItem;
        }
        else if (interactableObjectList.Count > 0)
        {
            // If there's only 1 item, then it returns the only object in the list
            return interactableObjectList[0];
        }
        else
        {
            // If the list is empty, then it returns null
            return null;
        }
    }

    // The function that handles movement itself
    public void Movement()
    {
        // Changing the move axis to a Vector3
        Vector3 moveDirection = new Vector3(ourPlayer.moveAxis.x, 0f, ourPlayer.moveAxis.y).normalized;

        // Checking if it's not empty
        if (ourPlayer.moveAxis != Vector2.zero)
        {
            // If the sprint key is held down, it multiplies the speed by the sprint multiplier
            // If not, it multiplies it by time and the player's speed stat
            if (PlayerController.instance.sprintHeldDown && moveDirection.z > 0)
            {
                moveDirection *= Time.deltaTime * ourPlayer.playerStats.stat[StatType.speed] * SprintMultiplier;
            }
            else
            {
                moveDirection *= Time.deltaTime * ourPlayer.playerStats.stat[StatType.speed];
            }

            // After multiplying it by the speed, it will transform the direction of movement into world space
            moveDirection = transform.TransformDirection(moveDirection);
            // If the player is falling, the y value is set to how fast the player is falling
            moveDirection.y = fallingSpeed;
        }

        // If the player is in the air, it will add one tenth of the gravity value to the falling speed
        if (!charController.isGrounded)
        {
            fallingSpeed -= (Gravity / 10) * Time.deltaTime;
            moveDirection.y = fallingSpeed;
        }
        else
        {
            if (jumpsRemaining != totalJumps)
            {
                jumpsRemaining = totalJumps;
            }
            
            if (fallingSpeed != 0f)
            {
                fallingSpeed = 0f;
            }
        }

        // If not, it checks if the player is trying to jump, then adds one tenth of the jump speed to the move
        if (ourPlayer.jumpHeldDown && (canJump && jumpsRemaining > 0))
        {
            canJump = false;
            jumpsRemaining--;   
            fallingSpeed = (JumpSpeed / 10);
            moveDirection.y = fallingSpeed;
        }
        else if (!ourPlayer.jumpHeldDown)
        {
            canJump = true;
            
        }
        
        // After everything is calculated, they player is moved based on the moveDirection
        charController.Move(moveDirection);
    }

    // The function that the useObject 
    public void UseObject()
    {
        // If there is an interactable object with code, it will interact with it
        if (interactableObject != null && interactableObject.GetComponent<Interactable>() != null)
        {
            interactableObject.GetComponent<Interactable>().Interact();
        }
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
        Debug.Log(damageTaken);
    }

    public void ChangeTemperature(float tempToAdd)
    {
        playerStats.AddToStat(StatType.temperature, tempToAdd);
        PlayerUI.instance.ChangeTemperature();
        // Debug.Log(playerStats.stat[StatType.temperature]);
    }
}