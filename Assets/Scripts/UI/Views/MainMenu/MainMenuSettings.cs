using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MainMenuSettings : MonoBehaviour
{

    [Header("Sound")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider sfxSlider;


    [Header("Mouse")]
    [SerializeField] private TextMeshProUGUI sensitivityValueTxt;
    [SerializeField] private Slider sensitivitySlider;

    SoundSettings soundSettings;
    MouseSensitivity mouseSensitivity;

    // ------------------------------------------------------------------- //
    void Start()
    {
        soundSettings = SaveReadManager.Instance.gameData.soundSettings;
        mouseSensitivity = SaveReadManager.Instance.gameData.mouseSensitivity;
        SetSliderValue();
        LoadVolume(); 
    }

    // ------------------------------------------------------------------- //
    void SetSliderValue()
    {
        masterSlider.value = soundSettings.masterVolume;
        musicSlider.value = soundSettings.musicVolume;
        uiSlider.value = soundSettings.uiVolume;
        sfxSlider.value = soundSettings.sfxVolume;
        sensitivitySlider.value = mouseSensitivity.sensitivity;
        sensitivityValueTxt.text = mouseSensitivity.sensitivity.ToString();
    }

    // ------------------------------------------------------------------- //
    void LoadVolume()
    {
        SetVolumeMaster(soundSettings.masterVolume);
        SetVolumeMusic(soundSettings.musicVolume);
        SetVolumeUi(soundSettings.uiVolume);
        SetVolumeSfx(soundSettings.sfxVolume);
    }

    // ======== SET VOLUME ========== //
    public void SetVolumeMaster(float volume)
    {
        soundSettings.masterVolume = volume;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeMusic(float volume)
    {
        soundSettings.musicVolume = volume;
        audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeUi(float volume)
    {
        soundSettings.uiVolume = volume;
        audioMixer.SetFloat("uiVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeSfx(float volume)
    {
        soundSettings.sfxVolume = volume;
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
    }

    public void ChangeSensetivity(float sensitivity)
    {
        // AI for å runde av med Math.Round.
        float newSensitivity = Mathf.Round(sensitivity * 100) / 100f;

        mouseSensitivity.sensitivity = newSensitivity;
        sensitivityValueTxt.text = newSensitivity.ToString();
    }

}
