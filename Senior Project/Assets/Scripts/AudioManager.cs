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

    public static float masterVolume = 10;
    public static float soundVolume = 10;
    public static float ambientVolume = 10;
    public static float musicVolume = 10;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static void SetMixerVolume(float value, AudioMixerType audioMixerType = AudioMixerType.Master)
    {
        instance.audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(Mathf.Clamp(value, 0.0001f, Mathf.Infinity)) * 20f);
        SetAudioMixerVolume(audioMixerType, value);
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

    public static void SetAudioMixerVolume(AudioMixerType audioMixerType = AudioMixerType.Master, float newVolume = 0)
    {
        switch (audioMixerType)
        {
            case AudioMixerType.Master: masterVolume = newVolume; break;
            case AudioMixerType.SoundEffect: soundVolume = newVolume; break;
            case AudioMixerType.Music: musicVolume = newVolume; break;
            case AudioMixerType.Ambient: ambientVolume = newVolume; break;
        }
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
