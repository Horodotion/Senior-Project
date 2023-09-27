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

        slider.onValueChanged.AddListener(OnValueChanged);
        // interval = (maxValue - minValue) / 10; 
    }

    public void OnValueChanged(float newValue)
    {
        currentValue = Mathf.Clamp(maxValue, minValue, minValue + (interval * slider.value));

        onSideInputEvent.Invoke();
    }

    public override void OnSideInput(float direction)
    {
        if (direction == 0)
        {
            return;
        }
        
        slider.value += Mathf.Sign(direction);
    }

    public void SetSliderValue(float referenceValue)
    {
        slider.value = referenceValue;
    }
}
