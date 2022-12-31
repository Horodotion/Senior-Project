using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorial : MonoBehaviour
{
    public GameObject tutorialCanvas;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            tutorialCanvas.GetComponent<TutorialUI>().Inititalize();
            tutorialCanvas.GetComponent<TutorialUI>().DashTutorial();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            tutorialCanvas.GetComponent<TutorialUI>().Deactivate();
        }
    }
}
