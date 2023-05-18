using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InterfaceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public UnityEvent onPointerDownEvent, onPointerUpEvent, onPointerEnteredEvent, onPointerExitedEvent;
    
    public Animator anim;
    [HideInInspector] public string highlighted = "Highlighted", pointerDown = "PointerDown", clicked = "Clicked"; 

    void Awake()
    {
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (anim != null)
        {
            anim.SetBool(highlighted, true);
        }

        if (MenuScript.currentMenu != null && MenuScript.currentMenu.selector != null)
        {
            MenuScript.currentMenu.MoveSelectorToButton(this);
        }

        onPointerEnteredEvent.Invoke();
        // Debug.Log("Entered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (anim != null)
        {
            anim.SetBool(highlighted, false);
        }

        onPointerExitedEvent.Invoke();
        // Debug.Log("Exited");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (anim != null)
        {
            anim.SetBool(pointerDown, true);
            anim.SetTrigger(clicked);
        }

        onPointerDownEvent.Invoke();
        // Debug.Log("Clicked");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (anim != null)
        {
            anim.SetBool(pointerDown, false);
            anim.ResetTrigger(clicked);
        }

        onPointerUpEvent.Invoke();
        // Debug.Log("Clicked");
    }

    public void UpdateAnimation()
    {

    }
}
