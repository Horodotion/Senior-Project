using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenuScript : MenuScript
{
    public static PauseMenuScript instance;
    public InterfaceButton resumeGameButton, retryLevelButton, optionsMenuButton, mainMenuButton, exitGameButton;
    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            exitGameButton.onPointerDownEvent.AddListener(() => Application.Quit());
            resumeGameButton.onPointerDownEvent.AddListener(() => GeneralManager.instance.UnPauseGame());
            mainMenuButton.onPointerDownEvent.AddListener(() => GeneralManager.ReturnToMainMenu());
            retryLevelButton.onPointerDownEvent.AddListener(() => GeneralManager.ReloadLevel());

            gameObject.SetActive(false);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
