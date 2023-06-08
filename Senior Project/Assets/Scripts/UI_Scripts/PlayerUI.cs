using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    [Header("Temperature")]
    public Slider temperatureSlider;
    [Header("Vignettes")]
    public VignetteController iceVingette;
    public VignetteController fireVingette;
    // public CanvasGroup iceVingette;
    // public CanvasGroup fireVingette;
    public Transform damageTextParent;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;

    [Header("Icicle Counts")]
    public Slider icicleCountSlider;
    public Image iciclePanel;
    private float maximumCharges;
    private float currentCharges;
    
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

    public void InitializeIcicles(Spell ourIcicle)
    {
        
    }

    public void ChangeIcicleCounter(Spell ourIcicle)
    {
        // if (ourIcicle.charges == ourIcicle.maximumCharges)
        // {
        //     return;
        // }

        // Debug.Log("Charge Amount = " + (ourIcicle.charges / ourIcicle.maximumCharges));
        iciclePanel.fillAmount = (float)ourIcicle.charges / (float)ourIcicle.maximumCharges;
        icicleCountSlider.value = 1 - (ourIcicle.rechargeTimer / ourIcicle.rechargeRate);
    }
}