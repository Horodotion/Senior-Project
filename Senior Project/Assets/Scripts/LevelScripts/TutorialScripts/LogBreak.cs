using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBreak : MonoBehaviour
{
    public GameObject tutorialCanvas;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            tutorialCanvas.GetComponent<TutorialUI>().Inititalize();
            tutorialCanvas.GetComponent<TutorialUI>().LogBreak();
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
