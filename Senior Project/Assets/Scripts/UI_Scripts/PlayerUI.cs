using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    public Slider temperatureSlider;
    public CanvasGroup iceVingette;
    public CanvasGroup fireVingette;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        temperatureSlider.minValue = PlayerController.instance.temperature.minimum;
        temperatureSlider.maxValue = PlayerController.instance.temperature.maximum;

        ChangeTemperature();
    }

    public void ChangeTemperature()
    {
        temperatureSlider.value = PlayerController.instance.temperature.stat;

        IndividualStat temp = PlayerController.instance.temperature;

        if (temp.stat > temp.maximum / 2)
        {
            fireVingette.alpha = (temp.stat - (temp.maximum / 2)) / (temp.maximum / 2);
        }
        else if (temp.stat < temp.minimum / 2)
        {
            iceVingette.alpha = (temp.stat - (temp.minimum / 2)) / (temp.minimum / 2);
        }
        else if (fireVingette.alpha != 0 || iceVingette.alpha != 0)
        {
            fireVingette.alpha = 0;
            iceVingette.alpha = 0;
        }
    }
}