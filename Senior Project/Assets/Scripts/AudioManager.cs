using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioMixerType
{
    SoundEffect,
    Music,
    Ambient,
    Master
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer audioMixer;
    public AudioMixerGroup master;
    public AudioMixerGroup soundEffect;
    public AudioMixerGroup ambient;
    public AudioMixerGroup music;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static void SetMixerVolume(float value, AudioMixerType audioMixerType = AudioMixerType.Master)
    {
        instance.audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(value) * 20f);
        // GetAudioMixerGroup(audioMixerType).audioMixer.SetFloat = value;
    }

    public static AudioMixerGroup GetAudioMixerGroup(AudioMixerType audioMixerType = AudioMixerType.Master)
    {
        return audioMixerType switch
        {
            AudioMixerType.SoundEffect => instance.soundEffect,
            AudioMixerType.Ambient => instance.ambient,
            AudioMixerType.Music => instance.music,
            AudioMixerType.Master => instance.master,
            _ => null,
        };
    }

    public static void PlaySoundAtLocation(AudioClip audioToPlay, Vector3 worldPosition, AudioMixerType audioMixerType = AudioMixerType.Master)
    {
        if (audioToPlay == null)
        {
            return;
        }

        GameObject newAudioClip = new ("Detached Audio Source");
        newAudioClip.transform.position = worldPosition;
        AudioSource audioSource = (AudioSource) newAudioClip.AddComponent(typeof(AudioSource));

        audioSource.clip = audioToPlay;
        audioSource.outputAudioMixerGroup = GetAudioMixerGroup(audioMixerType);
        audioSource.Play();

        Destroy(newAudioClip, audioToPlay.length * (Time.timeScale < 0.009999999 ? 0.01f : Time.timeScale));
    }
}
