using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InterfaceSlider : InterfaceButton
{
    public Slider slider;
    public float minValue, maxValue, interval, currentValue;

    public override void Awake()
    {
        base.Awake();
        slider = GetComponentInChildren<Slider>();
        // interval = (maxValue - minValue) / 10; 
    }

    public override void OnSideInput(float direction)
    {
        if (direction == 0)
        {
            return;
        }
        
        slider.value += Mathf.Sign(direction);
        currentValue = Mathf.Clamp(maxValue, minValue, minValue + (interval * slider.value));

        onSideInputEvent.Invoke();
    }

    public void SetSliderValue(float referenceValue)
    {
        slider.value = referenceValue;
    }
}
