using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Audio;
using Unity.VisualScripting;

public class PauseView : MonoBehaviour
{
    public static event Action OnSettingChange;

    [Header("GameObject")]
    [SerializeField] private GameObject viewContainer;
    [SerializeField] private GameObject allButtonsView;
    [SerializeField] private GameObject settingsView;
    [SerializeField] private GameObject controlsView;

    [Header("changeSong button")]
    [SerializeField] private Button newSongBtn;

    [Header("First View buttons")]
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button controlsBtn;
    [SerializeField] private Button settingsBtn;

    [Header("Controller view")]
    [SerializeField] private Button controllerViewToFrontBtn;
    [SerializeField] private Button keyboardBtn;
    [SerializeField] private Button gamepadBnt;
    [SerializeField] private GameObject keyboardControlsList;
    [SerializeField] private GameObject gamepadControlsListView;

    [Header("Settings view")]
    GameData gameData;
    [SerializeField] private Button backBtnSettings;
    [SerializeField] private Toggle isSpinningToggle;

    [Header("Sound")]
    SoundSettings soundSettings;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Sensetivity")]
    MouseSensitivity mouseSensitivity;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueTxt;

    // --------------------------- //
    void Awake()
    {
        gameData = SaveReadManager.Instance.gameData;
        soundSettings = SaveReadManager.Instance.gameData.soundSettings;
        mouseSensitivity = SaveReadManager.Instance.gameData.mouseSensitivity; 
    }

    // --------------------------- //
    void Start()
    {
        SetUpClickEventes();
        SetupSliderValue();
        SetupChangeListners();
    }

    // --------------------------- //
    void OnEnable()
    {
        PlayerScript.PausePress += OpenPauseMenu;
    }

    void OnDisable()
    {
        PlayerScript.PausePress -= OpenPauseMenu;
    }

    // --------------------------- //
    private void OpenPauseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        viewContainer.SetActive(true);
    }

    // --------------------------- //
    void SetupSliderValue()
    {
        isSpinningToggle.isOn = gameData.turnOffRotation;
        masterSlider.value = soundSettings.masterVolume;
        musicSlider.value = soundSettings.musicVolume;
        uiSlider.value = soundSettings.uiVolume;
        sfxSlider.value = soundSettings.sfxVolume;
        sensitivitySlider.value = mouseSensitivity.sensitivity;
        sensitivityValueTxt.text = mouseSensitivity.sensitivity.ToString();
    }

    // --------------------------- //
    void SetUpClickEventes()
    {

        // --- first view in pause menu --- //

        newSongBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.SelectNewSong();
        });

        mainMenuBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.LeaveCurrentRun(); 
        });

        continueBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.ChangeGameState(GameState.Playing);
            viewContainer.SetActive(false);
        });

        controlsBtn.onClick.AddListener(() =>
        {
            allButtonsView.SetActive(false);
            controlsView.SetActive(true);
        });

        settingsBtn.onClick.AddListener(() =>
        {
            allButtonsView.SetActive(false);
            settingsView.SetActive(true);
        });

        // --- controller view clicks --- //

        controllerViewToFrontBtn.onClick.AddListener(() =>
        {
            controlsView.SetActive(false);
            allButtonsView.SetActive(true);
        });

        keyboardBtn.onClick.AddListener(() =>
        {
            keyboardControlsList.SetActive(true);
            gamepadControlsListView.SetActive(false);
        });

        gamepadBnt.onClick.AddListener(() =>
        {
            keyboardControlsList.SetActive(false);
            gamepadControlsListView.SetActive(true);
        });

        // --- settings view --- //

        isSpinningToggle.onValueChanged.AddListener((isOn) =>
        {
            gameData.turnOffRotation = isOn;
            SaveReadManager.Instance.SaveData();
            OnSettingChange?.Invoke();
            
        });

        backBtnSettings.onClick.AddListener(() =>
        {
            settingsView.SetActive(false);
            SaveReadManager.Instance.SaveData();
            OnSettingChange?.Invoke();
            allButtonsView.SetActive(true);
        });
    }

    // ---------------------------------------------------------------------------- //
    public void SetupChangeListners()
    {
        masterSlider.onValueChanged.AddListener((float volume) =>
        {
            soundSettings.masterVolume = volume;
            audioMixer.SetFloat("masterVolume",  Mathf.Log10(volume) * 20);  
        });

        musicSlider.onValueChanged.AddListener((float volume) =>
        {
            soundSettings.musicVolume = volume;
            audioMixer.SetFloat("musicVolume",  Mathf.Log10(volume) * 20);  
        });

        sfxSlider.onValueChanged.AddListener((float volume) =>
        {
            soundSettings.sfxVolume = volume;
            audioMixer.SetFloat("sfxVolume",  Mathf.Log10(volume) * 20);    
        });

        uiSlider.onValueChanged.AddListener((float volume) =>
        {
            soundSettings.uiVolume = volume;
            audioMixer.SetFloat("uiVolume",  Mathf.Log10(volume) * 20);    
        });

        sensitivitySlider.onValueChanged.AddListener((float sensitivity) =>
        {
            float newSensitivity = Mathf.Round(sensitivity * 100) / 100f;
            mouseSensitivity.sensitivity = newSensitivity;
            sensitivityValueTxt.text = newSensitivity.ToString();
        });
    }    
}
