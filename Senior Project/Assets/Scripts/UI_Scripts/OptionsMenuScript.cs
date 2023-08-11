using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class OptionsMenuScript : MenuScript
{
    public static OptionsMenuScript instance;
    [SerializeField] private VolumeProfile profile;
    private LiftGammaGain lgg;

    public AudioMixer mixer;

    [SerializeField] private InterfaceButton backButton, resetGainButton;
    [SerializeField] private Slider gainSlider;

    [SerializeField] private float defaultGain = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            backButton.onPointerDownEvent.AddListener(() => GeneralManager.instance.CloseOptions());
            resetGainButton.onPointerDownEvent.AddListener(() => instance.ResetGain());

            profile.TryGet(out lgg);
            gameObject.SetActive(false);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void GetGain()
    {
        if (lgg != null)
        {
            gainSlider.enabled = true;
            gainSlider.value = lgg.gain.value.w;
        }
        else gainSlider.enabled = false;
    }

    public void SetGain(float val)
    {
        if (lgg != null)
        {
            lgg.gain.Override(new Vector4(lgg.gain.value.x, lgg.gain.value.y, lgg.gain.value.z, val));
        }
        else gainSlider.enabled = false;
    }

    public void ResetGain()
    {
        SetGain(defaultGain - 1f);
        if (gainSlider != null)
        {
            gainSlider.value = lgg.gain.value.w;
        }
    }


    // I'd much rather use a switch and an enum for this, but I don't want to create my own enum since it would conflict with yours
    // One float per function works nicer with default Unity sliders anyway, but once you make the enum and everything, by all means condense these functions as you see fit

    public void SetMasterVolume(float val)
    {
        if (mixer != null)
        {
            mixer.SetFloat("Master", Mathf.Log10(val) * 20f);
        }
    }
    public void SetSoundEffectVolume(float val)
    {
        if (mixer != null)
        {
            mixer.SetFloat("SoundEffect", Mathf.Log10(val) * 20f);
        }
    }
    public void SetMusicVolume(float val)
    {
        if (mixer != null)
        {
            mixer.SetFloat("Music", Mathf.Log10(val) * 20f);
        }
    }
    public void SetAmbientVolume(float val)
    {
        if (mixer != null)
        {
            mixer.SetFloat("Ambient", Mathf.Log10(val) * 20f);
        }
    }


    //public float AudioMixerVolume() // Returns the volume of either the Master or SoundEffect mixer group (whichever is lower), between 1 and 0
    //{
    //    float vol = 0f;

    //    if (instance.mixer != null)
    //    {
    //        instance.mixer.GetFloat("Master", out float master);
    //        instance.mixer.GetFloat("SoundEffect", out float sfx);
    //        Debug.Log($"Master: {master}, SoundEffect: {sfx}");

    //        if (master <= sfx)
    //        {
    //            vol = master += 80f;
    //        }
    //        else
    //        {
    //            vol = sfx += 80f;
    //        }

    //        vol /= 80f;
    //    }
    //    Debug.Log($"{vol}");
    //    return vol;
    //}
}
