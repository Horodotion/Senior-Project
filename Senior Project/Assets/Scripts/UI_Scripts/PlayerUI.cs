using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    public PlayerController player;

    // public Slider healthSlider;
    public Slider temperatureSlider;
    
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
        player = PlayerController.instance;

        // healthSlider.maxValue = player.playerStats.maxStat[StatType.health];

        temperatureSlider.minValue = player.playerStats.minStat[StatType.temperature];
        temperatureSlider.maxValue = player.playerStats.maxStat[StatType.temperature];

        ChangeHealth();
        ChangeTemperature();
    }

    public void ChangeHealth()
    {
        // healthSlider.value = player.playerStats.stat[StatType.health];
    }

    public void ChangeTemperature()
    {
        temperatureSlider.value = player.playerStats.stat[StatType.temperature];
    }
}