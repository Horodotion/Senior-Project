using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class OptionsMenuScript : MonoBehaviour
{
    public static OptionsMenuScript instance;
    [SerializeField] private VolumeProfile profile;
    private LiftGammaGain lgg;

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private InterfaceButton backButton, resetGainButton;
    [SerializeField] private Slider gainSlider, masterVolumeSlider;

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

    public void SetVolume(float val)
    {
        if (mixer != null)
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(val) * 20f);
        }
    }
}
