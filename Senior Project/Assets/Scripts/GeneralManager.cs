using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum Faction
{
    Player,
    Enemy,
    Wall,
    Terrain,
    other
}

[System.Serializable] public class EventFlag
{
    public string eventString; // A string to name the event in the hierarchy, planned to be unused
    public int eventID; // The int that will determine its place in the dictionary
    [HideInInspector] public bool eventTriggered; // A bool for if the variable has been activated or not
    public List<int> eventRequirements;
}


// This is the basic manager that handles transferring data between scenes and switching scenes.
// It should also contain static calculations or methods
public class GeneralManager : MonoBehaviour
{
    //A static reference to the GeneralManager
    public static GeneralManager instance;
    public static bool isGameRunning = false;
    public static bool hasGameStarted = true;

    // Variables for the event flags
    public Dictionary<int, EventFlag> eventFlags = new Dictionary<int, EventFlag>(); // The dictionary of event flagss that will be referenced by game objects

    // Variables solely for the text box in the game
    // public GameObject textBoxObject;
    // public TMP_Text textToDisplay;
    // [HideInInspector] public TextBoxText textBoxText;
    // [HideInInspector] public bool typing = false;

    void Awake()
    {
        // This if statement checks if there is a general manager
        // If it finds no manager, it becomes the manager. If not, it destroys itself.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // eventFlags = NewEventDictionary(eventList); // Wires up the dictionary of event flags.
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        UnPauseGame();
    }

    // Loads a scene by its build index
    public static void LoadLevel(int levelToLoad)
    {
        //SpawnManager.instance.TurnOffEverything();
        SceneManager.LoadScene(levelToLoad);
        GeneralManager.instance.UnPauseGame();
        PathLight.ClearPath();

        if (levelToLoad > 0)
        {
            hasGameStarted = true;
        }
    }

    public static void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void LoadCheckPoint()
    {
        SpawnManager.instance.TurnOffEverything();
        PlayerPuppet puppet = PlayerController.puppet;

        puppet.ResetStats();
        puppet.charController.enabled = false;

        if (Checkpoint.currentCheckpoint != null)
        {
            puppet.transform.position = Checkpoint.currentCheckpoint.transform.position;
        }
        else
        {
            puppet.transform.position = Checkpoint.playerSpawn;
        }

        puppet.transform.localEulerAngles = Checkpoint.playerLookDirection;
        puppet.cameraObj.transform.localEulerAngles = new Vector3(puppet.transform.forward.x, 0f, 0f);
        puppet.lookRotation = new Vector3(0f, puppet.transform.localEulerAngles.y, 0f);
        PlayerController.instance.lookAxis = Vector2.zero;

        puppet.charController.enabled = true;
        GeneralManager.instance.UnPauseGame();
    }

    public static void ReloadLevel()
    {
        PlayerController.instance.temperature.ResetStat();

        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public static void ReturnToMainMenu()
    {
        LoadLevel(0);

        if (PauseMenuScript.instance != null)
        {
            PauseMenuScript.instance.gameObject.SetActive(false);
        }        

        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.gameObject.SetActive(false);
        }  

        Cursor.lockState = CursorLockMode.None;
        isGameRunning = false;
        hasGameStarted = false;

        PlayerController.ourPlayerState = PlayerState.inMenu;
        PlayerController.instance.temperature.ResetStat();
    }

    public void PauseGame()
    {
        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.gameObject.SetActive(false);
        }  

        if (PauseMenuScript.instance != null)
        {
            PauseMenuScript.instance.gameObject.SetActive(true);
        }  

        PlayerController.ourPlayerState = PlayerState.inMenu;
        Cursor.lockState = CursorLockMode.None;
        isGameRunning = false;
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        if (PlayerUI.instance != null)
        {
            PlayerUI.instance.gameObject.SetActive(true);
        } 

        if (PauseMenuScript.instance != null)
        {
            PauseMenuScript.instance.gameObject.SetActive(false);
        }  

        if (GameOverMenuScript.instance != null)
        {
            GameOverMenuScript.instance.gameObject.SetActive(false);
        }
        
        // textBoxText = null;
        // textBoxObject.SetActive(false);

        PlayerController.ourPlayerState = PlayerState.inGame;
        Cursor.lockState = CursorLockMode.Locked;
        isGameRunning = true;
        Time.timeScale = 1f;
    }

    public void WinGame()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 3)
        {
            OpenWinMenu();
        }
        else if (LoadNextLevelScript.instance != null)
        {
            LoadNextLevelScript.instance.activeLoadingZone = true;
        }
    }

    public void OpenWinMenu()
    {
        if (GameOverMenuScript.instance == null)
        {
            return;
        }
        
        MenuScript.SwapToMenu(GameOverMenuScript.instance.gameObject, PlayerUI.instance.gameObject);
        MenuScript.SwapToMenu(GameOverMenuScript.instance.winPanel, GameOverMenuScript.instance.losePanel);

        PlayerController.ourPlayerState = PlayerState.inMenu;
        Cursor.lockState = CursorLockMode.None;
        isGameRunning = false;
        hasGameStarted = false;
        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        if (GameOverMenuScript.instance == null)
        {
            return;
        }

        MenuScript.SwapToMenu(GameOverMenuScript.instance.gameObject, PlayerUI.instance.gameObject);
        MenuScript.SwapToMenu(GameOverMenuScript.instance.losePanel, GameOverMenuScript.instance.winPanel);

        PlayerController.ourPlayerState = PlayerState.inMenu;
        Cursor.lockState = CursorLockMode.None;
        isGameRunning = false;
        hasGameStarted = false;
        Time.timeScale = 0f;
    }

    // This checks off the flag for an event, and triggers other events to be active if it's able to be
    public void SetEventFlag(int flagToTrigger)
    {
        if (eventFlags[flagToTrigger].eventRequirements.Count > 0)
        {
            bool allRequirementsActivated = true;

            for (int i = 0; i < eventFlags[flagToTrigger].eventRequirements.Count; i++)
            {
                if (eventFlags[i].eventTriggered == false)
                {
                    allRequirementsActivated = false;
                    break;
                }
            }

            if (allRequirementsActivated)
            {
                eventFlags[flagToTrigger].eventTriggered = true;
            }
        }
        else
        {
            eventFlags[flagToTrigger].eventTriggered = true;
        }
    }


    // A text system, currently unused
    /*---------------------------------------------------------------------------------------------------

    public void ActivateTextBox(TextBoxText newText)
    {
        if (textBoxText == null)
        {
            textBoxText = newText;
            Time.timeScale = 0f;
            textBoxObject.SetActive(true);
            textToDisplay.text = "";
            if (textBoxText.currentLine == textBoxText.textLines.Count)
            {
                textBoxText.currentLine = 0;
            }
            
            StartCoroutine(TypeText(textBoxText.textLines[textBoxText.currentLine]));
        }
        else if (textBoxText != null && textBoxText.currentLine != textBoxText.textLines.Count)
        {
            if (typing == true)
            {
                StopAllCoroutines();
                if (textBoxText.stacking == false)
                {
                    textToDisplay.text = LineBreaker(textBoxText.textLines[textBoxText.currentLine]);
                }
                else
                {
                    textToDisplay.text += LineBreaker(textBoxText.textLines[textBoxText.currentLine]);
                }
                
                textBoxText.currentLine++;
                typing = false;
            }
            else
            {
                StartCoroutine(TypeText(textBoxText.textLines[textBoxText.currentLine]));
            }
        }
        else
        {
            textBoxText.currentLine = 0;
            UnPauseGame();
        }
    }
    
    public IEnumerator TypeText(string textToType)
    {
        if (textBoxText.stacking == false)
        {
            textToDisplay.text = "";
        }
        
        typing = true;

		foreach (char c in LineBreaker(textToType)) 
		{
			textToDisplay.text += c;
			yield return new WaitForSecondsRealtime(0.02f);
		}

        textBoxText.currentLine++;
        typing = false;
    }
    
    public string LineBreaker(string inputText)
    {
        string newText = inputText.Replace(";", System.Environment.NewLine);
        return newText;
    }
    ---------------------------------------------------------------------------------------------------*/
}