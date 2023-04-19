using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenuScript : MenuScript
{
    public static GameOverMenuScript instance;

    public GameObject winPanel;
    public GameObject losePanel;

    public InterfaceButton loadCheckPointButton, retryLevelButton, mainMenuButton, exitGameButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            exitGameButton.onPointerDownEvent.AddListener(() => Application.Quit());
            mainMenuButton.onPointerDownEvent.AddListener(() => GeneralManager.ReturnToMainMenu());
            retryLevelButton.onPointerDownEvent.AddListener(() => GeneralManager.ReloadLevel());
            loadCheckPointButton.onPointerDownEvent.AddListener(() => GeneralManager.LoadCheckPoint());

            gameObject.SetActive(false);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
