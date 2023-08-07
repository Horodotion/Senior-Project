using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public EventFlag eventFlag;
    // public static int totalCollectiblesGrabbed;
    public bool enemyCollectible;
    public string messageToDisplay = "Collectible";

    void Start()
    {
         if (GeneralManager.HasEventBeenTriggered(eventFlag))
        {
            gameObject.SetActive(false);
        }
        else
        {
            GeneralManager.AddEventToDict(eventFlag);
        }
    }

    public void PressToCollect()
    {
        if (enemyCollectible)
        {
            return;
        }

        Collect();
    }

    public void Collect()
    {
        GeneralManager.SetEventFlag(eventFlag);

        gameObject.SetActive(false);
        // totalCollectiblesGrabbed++;
    }
}
