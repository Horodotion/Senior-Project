using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    // Variables for the event flags
    public Dictionary<int, EventFlag> eventFlags = new Dictionary<int, EventFlag>(); // The dictionary of event flagss that will be referenced by game objects

    // Variables solely for the text box in the game
    // public GameObject uiPanel;
    public GameObject textBoxObject;
    public TMP_Text textToDisplay;
    [HideInInspector] public TextBoxText textBoxText;
    [HideInInspector] public bool typing = false;

    void Awake()
    {
        // This if statement checks if there is a general manager
        // If it finds no manager, it becomes the manager. If not, it destroys itself.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

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

    public static void LoadZone(int levelToLoad)
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        textBoxText = null;
        textBoxObject.SetActive(false);

        Time.timeScale = 1f;
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
}