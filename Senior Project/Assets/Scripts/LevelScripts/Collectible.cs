using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public EventFlag eventFlag;
    // public static int totalCollectiblesGrabbed;
    // public bool enemyCollectible;
    public string messageToDisplay = "Collectible";
    public GameObject ourPoof;
    public AudioClip ourPing;

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

    public void Collect()
    {
        GeneralManager.SetEventFlag(eventFlag);
        GeneralManager.totalCollectiblesCounter++;
        Instantiate(ourPoof, transform.position, Quaternion.identity);
        AudioManager.PlaySoundAtLocation(ourPing, transform.position, AudioMixerType.SoundEffect);
        gameObject.SetActive(false);
    }
}
