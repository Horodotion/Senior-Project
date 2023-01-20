using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyType
{
    noKeyRequired,
    generator_L,
    generator_T,
    generator_Z,
    door,
    other
}

public class KeyInteractable : Interactable
{
    public Key ourKey;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Interact();
        }
    }

    public override void Interact()
    {
        if (PlayerController.instance.keyRing.ContainsKey(ourKey.key))
        {
            if (PlayerController.instance.keyRing[ourKey.key] == null)
            {
                Key newKey = Instantiate(ourKey);
                PlayerController.instance.keyRing[ourKey.key] = newKey;
            }
            else
            {
                PlayerController.instance.keyRing[ourKey.key].stackCount++;
            }
        }
        else
        {
            Key newKey = Instantiate(ourKey);
            PlayerController.instance.keyRing.Add(ourKey.key, newKey);
        }

        // if(PlayerController.puppet.interactableObjectList.Contains(gameObject))
        // {
        //     PlayerController.puppet.interactableObjectList.Remove(gameObject);
        // }
        Destroy(gameObject);
    }
}