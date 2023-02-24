using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MenuScript
{
    public InterfaceButton resumeGameButton, retryLevelButton, mainMenuButton, exitGameButton;

    void Awake()
    {
        exitGameButton.onPointerDownEvent.AddListener(() => Application.Quit());
        resumeGameButton.onPointerDownEvent.AddListener(() => GeneralManager.instance.UnPauseGame());
        mainMenuButton.onPointerDownEvent.AddListener(() => GeneralManager.LoadLevel(0));
        retryLevelButton.onPointerDownEvent.AddListener(() => GeneralManager.ReloadLevel());
    }

    // public void Start()
    // {
    //     if (!GeneralManager.isGameRunning)
    //     {
    //         gameObject.SetActive(false);
    //     }
    // }   
}
