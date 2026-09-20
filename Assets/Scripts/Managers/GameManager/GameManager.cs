using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    GameStateMachine gameState;
    public static Action OnSkinChange;

    [Header("Maps")]
    public LevelDataSO currentLevel;
    public LevelDataSO[] allLevels;

    [Header("Skins")]
    public SkinDataSO currentSkin;
    public SkinDataSO[] allSkins;
    public Sprite rewardUnknown;

    GameData gameData;

    void OnEnable()
    {
        GameStateMachine.OnStateExit += ExitState;
        GameStateMachine.OnStateEnter += EnterState;

        PlayerScript.OnPlayerDeath += ResetLevelEvent;
        PlayerScript.PausePress += CheckIfCanPause;
    }

    // -------------------------------------------- //
    void OnDisable()
    {
        GameStateMachine.OnStateExit -= ExitState;
        GameStateMachine.OnStateEnter -= EnterState;

        PlayerScript.OnPlayerDeath -= ResetLevelEvent;
        PlayerScript.PausePress -= CheckIfCanPause;
    }

    // -------------------------------------------- //
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            gameState = GetComponent<GameStateMachine>();
            Application.targetFrameRate = 100;
            QualitySettings.vSyncCount = 0;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // -------------------------------------------- //
    void Start()
    {
        gameState.SetState(GameState.MainMenu);
        gameData = SaveReadManager.Instance.gameData;
        SetCurrentSkin(gameData.currentSkinID);
    }

    // -------------------------------------------- //
    private void ResetLevelEvent() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // -------------------------------------------- //
    void CheckIfCanPause()
    {
        if(gameState.currentState == GameState.Loading)
        {
            return;
        } else
        {
            ChangeGameState(GameState.Paused);
        }
    }

    //======= methods =============//

    IEnumerator SlowTime(float duration)
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(duration);

        if(gameState.currentState == GameState.LevelComplete)
        {
            Time.timeScale = 0f;
        }
    }

    //====== EVENTS ======//
    // ------------state events-------------------------------- //
    void ExitState(GameState exitState)
    {
        switch(exitState)
        {
            case GameState.Loading:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 1f;
                break;
            case GameState.LevelComplete:
                Time.timeScale = 1f;
                break;
        }
    }

    // -------------------------------------------- //
    void EnterState(GameState newState)
    {
        switch(newState)
        {
            case GameState.Loading:
                Time.timeScale = 0f;
                break;
            case GameState.MainMenu:
                currentLevel = null;
                break;
            case GameState.Playing:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked; 
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None; 
                Cursor.visible = true;
                break;
            case GameState.LevelComplete:
                StartCoroutine(SlowTime(0.5f));
                Cursor.lockState = CursorLockMode.None; 
                Cursor.visible = true;
                break;
        }
    }

    //--------pause menu events--------------//
    public void LeaveCurrentRun()
    {
        LoadingScreen.Instance.SwitchToLevel(0, GameState.MainMenu);
    }

    public void SetPlayingState()
    {
        gameState.SetState(GameState.Playing);
    }

    //-----------events fra TimeManager-----------------//

    public void ChangeGameState(GameState newState)
    {
        gameState.SetState(newState);
    }

    public void OnRestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameState.SetState(GameState.Playing);
    }

    //---------helper methods -------------//


    public LevelDataSO NextLevelDataSO()
    {
        int currentIndex = Array.IndexOf(allLevels, currentLevel);
        int nextIndex = currentIndex + 1; 

        if(nextIndex < allLevels.Length)
        {
            return allLevels[nextIndex];
        }        

        return null;
    }

    // -------------------------------------------- //
    private Sprite CalculateMedal(float time, LevelDataSO currentMap)
    {

        if (time <= currentMap.goldTime)
        {
            return currentMap.goldIcon;
        }
        if (time <= currentMap.silverTime)
        {
            return currentMap.silverIcon;
        } else
        {
            return currentMap.bronzeIcon;
        }
    }

    // -------------------------------------------- //
    private Sprite CalculateRewardImg(float time, LevelDataSO currentMap)
    {
        bool isSkinUnlcoked = gameData.unlockedSkins.Contains(currentMap.skinSO.skinID);

        if(isSkinUnlcoked)
        {
            return currentMap.skinSO.rewardOwned;
        }
        
        if(time <= currentMap.goldTime)
        {
            return currentMap.skinSO.rewardSkin;
        } else
        {
            return rewardUnknown;
        }
    }

    // -------------------------------------------- //
    public LevelComplete ModelDataLevelComplete(float runTime)
    {
        LevelDataSO currentMap = currentLevel;
        LevelData levelData = gameData.FindLevelData(currentLevel.levelID);
        bool notZeroTime = levelData.bestTime != 0f;

        return new LevelComplete
        {
            levelName = currentMap.levelName,
            currentRunTime = runTime,
            bestTime = levelData.bestTime,
            isNewRecord = runTime <= levelData.bestTime || levelData.bestTime == 0f,
            medalSprite = CalculateMedal(runTime, currentMap),
            rewardSprite = CalculateRewardImg(runTime, currentMap),
            isNextLevelOpen = runTime <= currentMap.silverTime || levelData.bestTime <= currentMap.silverTime && notZeroTime, 
            nextLevel = NextLevelDataSO(),
            goldTimeTxt = Utility.ConvertTime(currentMap.goldTime),
            silverTimeTxt = Utility.ConvertTime(currentLevel.silverTime)
        };
    }

    // -------------------------------------------- //
    public LevelCard ModelDataLevelCard(LevelDataSO level)
    {
        LevelData levelData = gameData.FindLevelData(level.levelID);

        return new LevelCard
        {
            levelName = level.levelName,
            bestTime = levelData.bestTime,
            goldTimeTxt = Utility.ConvertTime(level.goldTime),
            silverTimeTxt = Utility.ConvertTime(level.silverTime),
            bronzeTimeTxt = Utility.ConvertTime(level.silverTime) + " +",
            selectedLevelSO = level,
            mapIcon = level.mapIcon,
            isLevelUnlocked = levelData.mapUnlcoked,
            medalSprite = CalculateMedal(levelData.bestTime, level),
            isThereBestTime = levelData.bestTime != 0f
        };
    }

    // -------------------------------------------- //
    public SkinToggle ModelDataSkinToggle(SkinDataSO skin)
    {
        bool isUnlocked = gameData.unlockedSkins.Contains(skin.skinID);

        return new SkinToggle
        {
            isSkinUnlocked = isUnlocked,
            selectedSkin = gameData.currentSkinID == skin.skinID,
            skinName = isUnlocked ? skin.skinName : "Locked",
            skinID = skin.skinID,
        };
    }

    // -------------------------------------------- //
    public void SetCurrentSkin(int skinID)
    {
        foreach (SkinDataSO skin in allSkins) 
        {
            if(skin.skinID == skinID)
            {
                currentSkin = skin;
                OnSkinChange?.Invoke();
            }  
        }

        gameData.currentSkinID = skinID;
        SaveReadManager.Instance.SaveData();
    }
}
