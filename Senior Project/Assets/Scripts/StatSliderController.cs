using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Add this into the GameObject that has a slider in a canvas
[RequireComponent(typeof(Slider))]
public class StatSliderController : MonoBehaviour
{
    
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void ResetAllValue(IndividualStat thatStat)
    {
        slider.minValue = thatStat.minimum;
        slider.maxValue = thatStat.maximum;
        slider.value = thatStat.stat;
    }

    public void ResetValue(float thatValue)
    {
        slider.value = thatValue;
    }
}
