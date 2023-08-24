using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable] public class ButtonList
{
    [HideInInspector] public List<InterfaceButton> buttonList;
    public GameObject selector;
    public InterfaceButton currentlySelectedButton;
    [HideInInspector] public int currentButtonID;

    public void SetupList(List<InterfaceButton> buttons)
    {
        buttonList = buttons;

        currentButtonID = 0;
        CycleThoughList(0);
    }

    public void CycleThoughList(int direction)
    {
        int newID = currentButtonID + direction;

        if (newID < 0)
        {
            currentButtonID = buttonList.Count - 1;
        }
        else if (newID >= buttonList.Count)
        {
            currentButtonID = 0;
        }
        else
        {
            currentButtonID += direction;
        }

        selector.transform.position = buttonList[currentButtonID].transform.position;
        currentlySelectedButton = buttonList[currentButtonID];
    }
    
    public void ClickButton()
    {
        currentlySelectedButton.onPointerDownEvent.Invoke();
    }

    public void BackButton()
    {
        currentlySelectedButton.onPointerUpEvent.Invoke();
    }
}


public abstract class MenuScript : MonoBehaviour
{
    [Header("Menu Scripts")]
    public static MenuScript currentMenu;
    public GameObject selector;
    public InterfaceButton currentlySelectedButton;
    public List<InterfaceButton> buttonList;
    [HideInInspector] public int currentButtonID;

    [Header("Optional Components")]
    public TMP_Text counterText;
    public MenuScript optionsMenu;

    public virtual void Start()
    {
        MenuScript.currentMenu = this;
        PlayerController.ourPlayerState = PlayerState.inMenu;

        if (GetComponent<CanvasGroup>() != null)
        {
            GetComponent<CanvasGroup>().alpha = 1;
        }

        ConnectPlayerToMenu();  
    }

    void OnEnable()
    {
        if (counterText != null)
        {
            counterText.text = GeneralManager.totalCollectiblesCounter + "/" + GeneralManager.instance.totalCollectibles;
        }

        // ConnectPlayerToMenu();
    }

    public virtual void ConnectPlayerToMenu()
    {
        if (selector == null)
        {
            selector = Instantiate(SpawnManager.instance.selectorPrefab);
            selector.transform.SetParent(currentMenu.transform);
        }

        currentButtonID = 0;
        selector.transform.position = buttonList[currentButtonID].transform.position;
        currentlySelectedButton = buttonList[currentButtonID];
        currentlySelectedButton.ButtonEnter();
    }

    public void CycleThoughList(int direction)
    {
        int newID = currentButtonID + direction;

        if (newID < 0)
        {
            currentButtonID = buttonList.Count - 1;
        }
        else if (newID >= buttonList.Count)
        {
            currentButtonID = 0;
        }
        else
        {
            currentButtonID += direction;
        }

        currentlySelectedButton.ButtonExit();
        selector.transform.position = buttonList[currentButtonID].transform.position;
        currentlySelectedButton = buttonList[currentButtonID];
        currentlySelectedButton.ButtonEnter();
    }

    public virtual void MoveSelectorToButton(InterfaceButton ourButton)
    {
        currentButtonID = buttonList.IndexOf(ourButton);
        selector.transform.position = buttonList[currentButtonID].transform.position;
        currentlySelectedButton = buttonList[currentButtonID];
    }

    public static void SwapToMenu(GameObject menuToGoTo, GameObject menuToTurnOff)
    {
        menuToGoTo.SetActive(true);
        menuToTurnOff.SetActive(false);
    }

    public static void ToggleMenu(GameObject menu)
    {
        if (menu.activeInHierarchy)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }

    public virtual void OpenOptionsMenu()
    {
        
    }
}
