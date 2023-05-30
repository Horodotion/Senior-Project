using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    public GameObject tutorialText;
    public TextMeshProUGUI tutorialString;

    private void Start()
    {
        WalkTutorial();
    }

    public void Inititalize()
    {
        tutorialText.SetActive(true);
    }

    public void Deactivate()
    {
        tutorialText.SetActive(false);
    }

    public void FireTutorial()
    {
        tutorialString.text = "Fire Tutorial / Temp Tutorial";
    }

    public void WalkTutorial()
    {
        tutorialString.text = "Walk Tutorial";
    }

    public void JumpTutorial()
    {
        tutorialString.text = "Jump Tutorial";
    }

    public void DublJumpTutorial()
    {
        tutorialString.text = "Double Jump Tutorial";
    }

    public void DashTutorial()
    {
        tutorialString.text = "Dash Tutorial";
    }

    public void ObeliskTutorial()
    {
        tutorialString.text = "Obelisk Tutorial";
    }

    public void LogBreak()
    {
        tutorialString.text = "The rock collapses!";
    }
}
