using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    public Slider temperatureSlider;
    public VignetteController iceVingette;
    public VignetteController fireVingette;
    // public CanvasGroup iceVingette;
    // public CanvasGroup fireVingette;
    public Transform damageTextParent;

    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    
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

        fireVingette.SetVignetteIntensity(PlayerController.puppet.fireMultiplier);
        iceVingette.SetVignetteIntensity(PlayerController.puppet.iceMultiplier);


        // if (PlayerController.puppet.fireMultiplier != 0)
        // {
        //     fireVingette.SetVignetteIntensity(PlayerController.puppet.fireMultiplier);
        // }
        // else if (PlayerController.puppet.iceMultiplier != 0)
        // {
        //     iceVingette.SetVignetteIntensity(PlayerController.puppet.iceMultiplier);
        // }
        // else if (fireVingette.intensity != 0 || iceVingette.intensity != 0)
        // {
        //     fireVingette.SetVignetteIntensity(0f);
        //     iceVingette.SetVignetteIntensity(0f);
        // }
    }

    public void ResetUI()
    {
        temperatureSlider.value = 0;
        fireVingette.SetVignetteIntensity(0f);
        iceVingette.SetVignetteIntensity(0f);
    }

    public void ActivateTutorialPanel(string textToChange)
    {
        textToChange = textToChange.Replace(";", System.Environment.NewLine);
        tutorialText.text = textToChange;
        tutorialPanel.SetActive(true);
    }

    public void DeactivateTutorialPanel()
    {
        tutorialText.text = "";
        tutorialPanel.SetActive(false);
    }
}