using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InterfaceSlider : InterfaceButton
{
    public Slider slider;


    public override void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        // slider.onValueChanged.AddListener(delegate {OnSliderUpdate();});
    }

    public override void OnSideInput(float direction)
    {
        if (direction == 0)
        {
            return;
        }
        Debug.Log(Mathf.Sign(direction));

        slider.value += Mathf.Sign(direction);
        onSideInputEvent.Invoke();
    }

    // public void OnSliderUpdate()
    // {
    //     onSideInputEvent.Invoke();
    // }
}
