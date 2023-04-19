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

        if (PlayerController.puppet.fireMultiplier != 0)
        {
            fireVingette.alpha = PlayerController.puppet.fireMultiplier;
        }
        else if (PlayerController.puppet.iceMultiplier != 0)
        {
            iceVingette.alpha = PlayerController.puppet.iceMultiplier;
        }
        else if (fireVingette.alpha != 0 || iceVingette.alpha != 0)
        {
            fireVingette.alpha = 0;
            iceVingette.alpha = 0;
        }
    }

    public void ResetUI()
    {
        temperatureSlider.value = 0;
        fireVingette.alpha = 0;
        iceVingette.alpha = 0;
    }
}