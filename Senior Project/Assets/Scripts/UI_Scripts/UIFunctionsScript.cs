using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class UIFunctionsScript : MonoBehaviour
{
    // A static reference to the UI functions
    public static UIFunctionsScript instance;

    // access components from gameobjects
    public Slider healthbarSlider;
    public Slider ammoSliderPrimary;
    public Slider ammoSliderSecondary;
    public Slider ammoSliderHeavy;
    [HideInInspector] public Slider ammoSliderActive;
    public TextMeshProUGUI ammoReserveSecondary;
    public TextMeshProUGUI ammoReserveHeavy;
    public Image reticle;
    public TextMeshProUGUI useItemText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetUseItemText(string name)
    {
        if (useItemText.gameObject.activeSelf == false)
        {
            useItemText.gameObject.SetActive(true);
        }
        useItemText.SetText(name);
    }

    public void TurnOffUseItemText()
    {
        useItemText.SetText("");
        if (useItemText.gameObject.activeSelf == true)
        {
            useItemText.gameObject.SetActive(false);
        }
    }

    public void Update_Healthbar(int health) // Updates the Health-bar Display with the input
    {
        //Sets the HP bar with the input value
        healthbarSlider.value = health;
    }

    public void Update_Reticle(float size, bool visible = true) //Updates the size and visibility of the reticle
    {
        if (visible)
        {
            reticle.enabled = true;
            reticle.rectTransform.sizeDelta = new Vector2(size, size);
        }
        else
        {
            reticle.enabled = false;
        }
    }

    /*
        public void UpdateChargeMeter(float charge) // Updates the Health-bar Display with the input
        {
            //Sets the HP bar with the input value
            chargeMeter.value = (charge * 100);
        }

        public void UpdateDis_Money(int cashMoney) // Updates the Money Counter Display with the input
        {
            //Turns the Money integer into a string and updates the UI
            money_Ctrl.SetText(cashMoney.ToString());
        }

        public void UpdateDis_Level(int Level) // Updates the Level Counter Display with the input
        {
            //Turns the Level integer into a string and updates the UI
            level_Ctrl.SetText(Level.ToString());
        }

        public void UpdateExp(int Level) // Updates the Level Counter Display with the input
        {
            //Turns the Level integer into a string and updates the UI
            exp_Ctrl.SetText(Level.ToString());
        }
    */
}