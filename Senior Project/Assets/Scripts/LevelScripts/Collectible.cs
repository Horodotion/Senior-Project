using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public EventFlag eventFlag;
    public static int totalCollectiblesGrabbed;
    public bool enemyCollectible;
    public string messageToDisplay = "Collectible";

    void Start()
    {
        GeneralManager.AddEventToDict(eventFlag);

        if (GeneralManager.HasEventBeenTriggered(eventFlag))
        {
            gameObject.SetActive(false);
        }
        else
        {

        }
    }

    public void Collect()
    {
        if (enemyCollectible)
        {
            return;
        }

        GeneralManager.SetEventFlag(eventFlag);

        gameObject.SetActive(false);
        totalCollectiblesGrabbed++;
    }

    
}
