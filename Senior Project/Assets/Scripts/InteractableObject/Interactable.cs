using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [HideInInspector] public PlayerController player;
    public string interactableText;
    public bool onContact = false;

    public virtual void OnTriggerEnter(Collider col)
    {

    }

    public virtual void HighlightObject(PlayerController playerHighlighting)
    {

    }

    public virtual void Interact()
    {
        
    }
}
