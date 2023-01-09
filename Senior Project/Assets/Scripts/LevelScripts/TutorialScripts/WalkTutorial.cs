using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTutorial : MonoBehaviour
{

    public GameObject tutorialCanvas;
    public GameObject player;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            tutorialCanvas.GetComponent<TutorialUI>().Deactivate();
        }
    }
}
