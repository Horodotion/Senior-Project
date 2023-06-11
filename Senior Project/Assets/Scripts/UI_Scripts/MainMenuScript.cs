using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuScript : MenuScript
{
    public InterfaceButton playGameButton, levelSelectButton, exitGameButton, optionsMenuButton;
    // public InterfaceButton[] levelButtons;
    // public GameObject levelSelectPanel;

    void Awake()
    {
        exitGameButton.onPointerDownEvent.AddListener(() => Application.Quit());
        playGameButton.onPointerDownEvent.AddListener(() => GeneralManager.LoadNextLevel()); //Debug.Log("Level Select"));
        // levelSelectButton.onPointerDownEvent.AddListener(() => ToggleMenu(levelSelectPanel));
    }

    public override void Start()
    {
        base.Start();

        GeneralManager.hasGameStarted = false;
        Cursor.lockState = CursorLockMode.None;

        PlayerController.ourPlayerState = PlayerState.inMenu;
        Time.timeScale = 0f;
    }
}
