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

    void Start()
    {
        healthbarSlider.maxValue = PlayerController.instance.playerStats.maxStat[StatType.health];
        Update_Healthbar((int)PlayerController.instance.playerStats.stat[StatType.health]);
        Hide_AmmoMeter();

        Debug.Log((int)PlayerController.instance.playerStats.stat[StatType.health]);

        // Sets the max for each ammo meter to the actual defined max. As I type this these are identical but better safe than sorry. Super hacky but functional
        ammoSliderPrimary.gameObject.transform.Find("MaxChargeText (TMP)").GetComponent<TextMeshProUGUI>().text = PlayerController.instance.primaryWeapon.maxAmmo.ToString();
        ammoSliderSecondary.gameObject.transform.Find("MaxLoadedText (TMP)").GetComponent<TextMeshProUGUI>().text = PlayerController.instance.secondaryWeapon.maxAmmo.ToString();
        ammoSliderHeavy.gameObject.transform.Find("MaxLoadedText (TMP)").GetComponent<TextMeshProUGUI>().text = PlayerController.instance.heavyWeapon.maxAmmo.ToString();

        // chargeMeter.maxValue = (PlayerController.ourPlayer.playerStats.maxStat[StatType.charge] * 100);
        // chargeMeter.value = 0;
        // UpdateDis_Money((int)PlayerController.ourPlayer.playerStats.stat[StatType.money]);
        // UpdateDis_Level((int)PlayerController.ourPlayer.playerStats.stat[StatType.level]);
        // UpdateExp((int)PlayerController.ourPlayer.playerStats.stat[StatType.experience]);
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

    public void Update_AmmoMeter(float ammo, Slider slider, bool makeVisible = false, bool updateReserve = false) // Updates the specified ammo meter with the input, and set visible if specified
    {
        //Sets the specified ammo bar with the input value
        slider.value = ammo;

        //Update reserve ammo counters if applicable
        if (updateReserve == true)
        {
            if (slider == ammoSliderSecondary)
            {
                ammoReserveSecondary.text = PlayerController.instance.secondaryWeapon.ammoInReserve.ToString();
            }
            if (slider == ammoSliderHeavy)
            {
                ammoReserveHeavy.text = PlayerController.instance.heavyWeapon.ammoInReserve.ToString();
            }
        }

        //Makes this ammo slider visible, and the others invisible, if specified
        if (makeVisible)
        {
            // A bit hacky but functional- a better programmer than I (aka most programmers) can probably optimize this
            Hide_AmmoMeter();
            if (slider != null)
            {
                ammoSliderActive = slider;
                slider.gameObject.SetActive(true);
            }
        }
    }

    public void Hide_AmmoMeter() //Disables all ammo meters
    {
        ammoSliderPrimary.gameObject.SetActive(false);
        ammoSliderSecondary.gameObject.SetActive(false);
        ammoSliderHeavy.gameObject.SetActive(false);
        ammoSliderActive = null;
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