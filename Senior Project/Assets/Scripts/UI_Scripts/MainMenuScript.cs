using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuScript : MenuScript
{
    public InterfaceButton playGameButton, levelSelectButton, exitGameButton, optionsMenuButton;
    public InterfaceButton[] levelButtons;
    public GameObject levelSelectPanel;

    void Awake()
    {
        exitGameButton.onPointerDownEvent.AddListener(() => Application.Quit());
        playGameButton.onPointerDownEvent.AddListener(() => Debug.Log("Level Select"));
        levelSelectButton.onPointerDownEvent.AddListener(() => ToggleMenu(levelSelectPanel));

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int copyI = i;
            levelButtons[copyI].onPointerDownEvent.AddListener(() => GeneralManager.LoadLevel(copyI + 1));
        }
    }

    public override void Start()
    {
        base.Start();

        GeneralManager.hasGameStarted = false;
        Cursor.lockState = CursorLockMode.None;
    }
}
