using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class LockInteractable : Interactable
{
    public EventFlag ourEvent = null;
    public KeyType requiredKey;
    public string newInteractText;
    public bool locked = true;

    void Start()
    {
        

        if (ourEvent != null)
        {
            if (!GeneralManager.instance.eventFlags.ContainsKey(ourEvent.eventID))
            {
                GeneralManager.instance.eventFlags.Add(ourEvent.eventID, ourEvent);
            }
            else if (GeneralManager.instance.eventFlags[ourEvent.eventID].eventTriggered)
            {
                Unlock();
            }
        }
    }

    public override void Interact()
    {
        if (locked && PlayerController.instance.keyRing.ContainsKey(requiredKey) && PlayerController.instance.keyRing[requiredKey] != null)
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

    public virtual void Unlock()
    {
        locked = false;
        interactableText = newInteractText;
        Debug.Log("Unlocked");

        // if (PlayerController.puppet.interactableObject == gameObject)
        // {
        //     UIFunctionsScript.instance.SetUseItemText(newInteractText);
        // }
    }
}
*/