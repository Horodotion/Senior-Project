using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SoundEffectScript : MonoBehaviour
{
    public AudioSource ourAudioSource;

    void Awake()
    {
        ourAudioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect()
    {
        

        // yield return new WaitForSeconds(secondsToWait);
    }
}
