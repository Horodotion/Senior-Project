using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuScript : MenuScript
{
    public static OptionsMenuScript instance;
    public MenuScript menuToReturnTo;
    public InterfaceButton backButton;
    public InterfaceSlider masterVolumeSlider, musicVolumeSlider, ambientVolumeSlider, sfxVolumeSlider, brightnessSlider;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            return;
        }

        instance = this;

        backButton.onPointerDownEvent.AddListener(() => GeneralManager.instance.CloseOptionsMenu());
        brightnessSlider.onSideInputEvent.AddListener(() => DustinsLightsScript.SetGain(brightnessSlider.slider.value));        
        masterVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(masterVolumeSlider.slider.value, AudioMixerType.Master));
        musicVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(musicVolumeSlider.slider.value, AudioMixerType.Music));
        ambientVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(ambientVolumeSlider.slider.value, AudioMixerType.Ambient));
        sfxVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(sfxVolumeSlider.slider.value, AudioMixerType.SoundEffect));

        gameObject.SetActive(false);
    }

    public void OpenOptionsMenu(MenuScript menu)
    {
        menuToReturnTo = menu;

    }
}
