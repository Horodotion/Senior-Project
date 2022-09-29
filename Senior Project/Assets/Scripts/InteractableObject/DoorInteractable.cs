using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This code is used for the collider that is open doors
public class DoorInteractable : LockInteractable
{


    //The door that has the animation
    [SerializeField] GameObject doors;

    //Can be deleted once the aniomation has arrived
    [HideInInspector] public Animator anim;
    public bool doorClosed;



    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("doorClosed", true);
        doorClosed = true;

        if (requiredKey == KeyType.noKeyRequired)
        {
            locked = false;
        }
    }

    public override void Interact()
    {
        if (!locked)
        {
            if (doorClosed == false)
            {
                anim.SetBool("doorClosed", true);
                doorClosed = true;
            }
            else
            {
                anim.SetBool("doorClosed", false);
                doorClosed = false;
            }
        }

        
        else if (locked && PlayerController.instance.keyRing.ContainsKey(requiredKey) && PlayerController.instance.keyRing[requiredKey] != null)
        {
            Unlock();

            if (ourEvent != null && GeneralManager.instance.eventFlags.ContainsKey(ourEvent.eventID))
            {
                GeneralManager.instance.SetEventFlag(ourEvent.eventID);
            }

            if (PlayerController.instance.keyRing[requiredKey].ConsumedOnUse == true)
            {
                PlayerController.instance.keyRing[requiredKey].stackCount--;
                if (PlayerController.instance.keyRing[requiredKey].stackCount <= 0)
                {
                    PlayerController.instance.keyRing[requiredKey] = null;
                }
            }
        }

    }

    public override void Unlock()
    {
        locked = false;
        interactableText = newInteractText;
        Debug.Log("Unlocked");

        if (PlayerController.puppet.interactableObject == gameObject)
        {
            UIFunctionsScript.instance.SetUseItemText(newInteractText);
        }
    }




    /*
    public override void OnTriggerEnter(Collider other)
    {

        if (other.tag.Equals("Player"))
        {
            Interact();
            if (!locked)
            {
                Interact();
            }
            
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (!locked && other.tag.Equals("Player"))
        {
            //Close door animation
            anim.SetBool("doorClosed", true);
            doorClosed = true;
        }
    }
    */
}
