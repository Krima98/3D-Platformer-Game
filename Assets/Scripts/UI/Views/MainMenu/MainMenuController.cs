using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject levelSelector;
    [SerializeField] private GameObject gameInfoView;

    [Header("Start menu buttons")]
    [SerializeField] Button playBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] Button gameInfoBtn;

    [Header("Settings buttons")]
    [SerializeField] Button backToMainMenuBtnSettings;

    [Header("LevelSelector buttons")]
    [SerializeField] Button backToMainMenuBtnLevelSelector;

    [Header("gameInfo buttons")]
    [SerializeField] Button backToMainMenuBtnGameInfo;


    // ------------------------------------------------------------------- //
    void Awake()
    {
        SetUpClickEvents();
    }

    // ------------------------------------------------------------------- //
    void SetUpClickEvents()
    {
        playBtn.onClick.AddListener(() =>
        {
            OpenNewView(mainMenu, levelSelector);
        });

        settingsBtn.onClick.AddListener(() =>
        {
            OpenNewView(mainMenu, settingsMenu);
        });

        gameInfoBtn.onClick.AddListener(() =>
        {
            OpenNewView(mainMenu, gameInfoView); 
        });

        exitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        backToMainMenuBtnSettings.onClick.AddListener(() =>
        {
            SaveReadManager.Instance.SaveData();
            OpenNewView(settingsMenu, mainMenu);
        });

        backToMainMenuBtnLevelSelector.onClick.AddListener(() =>
        {
            OpenNewView(levelSelector, mainMenu);
        });

        backToMainMenuBtnGameInfo.onClick.AddListener(() =>
        {
            OpenNewView(gameInfoView, mainMenu);
        });

        // presentation

    }

    //==== Methods ====//

    void OpenNewView(GameObject currentActive, GameObject newActive)
    {
        currentActive.SetActive(false);
        newActive.SetActive(true);
    }

}
