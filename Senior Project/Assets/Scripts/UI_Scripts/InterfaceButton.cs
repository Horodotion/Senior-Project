using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InterfaceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public UnityEvent onPointerDownEvent, onPointerUpEvent, onPointerEnteredEvent, onPointerExitedEvent;
    
    public Animator anim;
    public Sprite idleImage;
    public Sprite hoverImage;
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
        ButtonEnter();
    }

    public void ButtonEnter()
    {
        if (anim != null)
        {
            anim.SetBool(highlighted, true);
        }

        if (MenuScript.currentMenu != null && MenuScript.currentMenu.selector != null)
        {
            MenuScript.currentMenu.MoveSelectorToButton(this);
            onPointerEnteredEvent.Invoke();
        }
        else
        {
            onPointerEnteredEvent.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ButtonExit();
    }

    public void ButtonExit()
    {
        if (anim != null)
        {
            anim.SetBool(highlighted, false);
        }

        onPointerExitedEvent.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonDown();
    }

    public void ButtonDown()
    {
        if (anim != null)
        {
            anim.SetBool(pointerDown, true);
            anim.SetTrigger(clicked);
        }

        onPointerDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonUp();
    }

    public void ButtonUp()
    {
        if (anim != null)
        {
            anim.SetBool(pointerDown, false);
            anim.ResetTrigger(clicked);
        }

        onPointerUpEvent.Invoke();
    }

    public void ChangeToIdleImage()
    {
        if (idleImage != null)
        {
            GetComponent<Image>().sprite = idleImage;
        }
    }

    public void ChangeToHoverImage()
    {
        if (hoverImage != null)
        {
            GetComponent<Image>().sprite = hoverImage;
        }
    }
}
