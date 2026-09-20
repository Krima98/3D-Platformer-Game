using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MapCompleteView : MonoBehaviour
{

    public static event Action<float> OnUIsetup;

    [Header("GameObject")]
    [SerializeField] private GameObject viewContainer;

    [Header("Stats elemets")]
    [SerializeField] private TextMeshProUGUI respawnsTxt;

    [Header("info")] 
    [SerializeField] private TextMeshProUGUI goldTimeTxt;
    [SerializeField] private TextMeshProUGUI silverTimeTxt;
    [SerializeField] private TextMeshProUGUI bronzeTimeTxt;

    [Header("img")]
    [SerializeField] private Image rewardImg;
    [SerializeField] private Image medalImg;

    [Header("YourScore elements")]
    [SerializeField] private TextMeshProUGUI currentTimeTxt;
    [SerializeField] private TextMeshProUGUI recordTxt;

    [Header("Navigation")]
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button nextLvlBtn;
    [SerializeField] private TextMeshProUGUI nextLvlButtonTxt;

    private float currentRunTime;

    void OnEnable()
    {
        RunTimer.onLevelFinish += OnMapComplete;
    }

    // ------------------------------------------------------------------- //
    void OnDisable()
    {
        RunTimer.onLevelFinish -= OnMapComplete;
    }

    // ------------------------------------------------------------------- //
    private void OnMapComplete(float runTime)
    {
        currentRunTime = runTime;
        viewContainer.SetActive(true);
        SetUpMapCompleteUI(currentRunTime);
    }

    // ------------------------------------------------------------------- //
    void SetUpMapCompleteUI(float currentRunTime)
    {

        LevelComplete levelModelData = GameManager.Instance.ModelDataLevelComplete(currentRunTime);

        currentTimeTxt.text = Utility.ConvertTime(currentRunTime);

        goldTimeTxt.text = "Gold: " + levelModelData.goldTimeTxt;
        silverTimeTxt.text = "Silver: " + levelModelData.silverTimeTxt;
        bronzeTimeTxt.text = "Bronze: " + levelModelData.silverTimeTxt + " +";

        medalImg.sprite = levelModelData.medalSprite;

        rewardImg.sprite = levelModelData.rewardSprite;
        recordTxt.text = levelModelData.isNewRecord ? "NEW RECORD" : "CURRENT RECORD: " + Utility.ConvertTime(levelModelData.bestTime);

        SetupButtons(levelModelData);

        OnUIsetup?.Invoke(currentRunTime);
    }

    // ------------------------------------------------------------------- //
    void SetupButtons(LevelComplete levelModelData)
    {
        restartBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.OnRestartLevel();
        });

        mainMenuBtn.onClick.AddListener(() => {
            GameManager.Instance.LeaveCurrentRun();
        });

        if(levelModelData.nextLevel == null)
        {
            nextLvlBtn.interactable = false;
            nextLvlButtonTxt.text = "LAST MAP";
            return;
        }

        if(levelModelData.isNextLevelOpen)
        {
            nextLvlBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.currentLevel = levelModelData.nextLevel;
                LoadingScreen.Instance.SwitchToLevel(levelModelData.nextLevel.buildIndex, GameState.Playing);
            });
        } else
        {
            nextLvlBtn.interactable = false;
            nextLvlButtonTxt.text = "NEED SILVER";
        }    
    }
}
