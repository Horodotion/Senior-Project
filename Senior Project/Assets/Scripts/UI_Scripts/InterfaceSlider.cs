using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Slider))]
public class InterfaceSlider : InterfaceButton
{
    public Slider slider;

    public override void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public override void OnSideInput(float direction)
    {
        if (direction == 0)
        {
            return;
        }

        slider.value += Mathf.Sign(direction);
        onSideInputEvent.Invoke();
    }
}
