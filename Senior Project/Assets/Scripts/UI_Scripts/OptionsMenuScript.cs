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

        brightnessSlider.slider.value = DustinsLightsScript.currentBrightness;
        brightnessSlider.onSideInputEvent.AddListener(() => DustinsLightsScript.SetGain(brightnessSlider.slider.value));

        masterVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(masterVolumeSlider.currentValue, AudioMixerType.Master));
        musicVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(musicVolumeSlider.currentValue, AudioMixerType.Music));
        ambientVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(ambientVolumeSlider.currentValue, AudioMixerType.Ambient));
        sfxVolumeSlider.onSideInputEvent.AddListener(() => AudioManager.SetMixerVolume(sfxVolumeSlider.currentValue, AudioMixerType.SoundEffect));

        gameObject.SetActive(false);
    }

    public override void Start()
    {
        base.Start();

        masterVolumeSlider.slider.value = AudioManager.masterVolume;
        musicVolumeSlider.slider.value = AudioManager.musicVolume;
        ambientVolumeSlider.slider.value = AudioManager.ambientVolume;
        sfxVolumeSlider.slider.value = AudioManager.soundVolume;
    }

    public void OpenOptionsMenu(MenuScript menu)
    {
        menuToReturnTo = menu;

    }
}
