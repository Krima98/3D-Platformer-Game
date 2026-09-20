using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorView : MonoBehaviour
{
    [SerializeField] RectTransform contentHolder;
    [SerializeField] LevelPrefab levelPrefab;

    [SerializeField] RectTransform contentHolderToggles;
    [SerializeField] ToggleGroup toggleGroup;
    [SerializeField] SkinTogglePrefab togglePrefab;

    // ------------------------------------------------------------------- //
    void Start()
    {
        SetUpSkinToggleList();
        SetUpLevelList();
    }

    // ------------------------------------------------------------------- //
    void SetUpLevelList()
    {   
        // levels view
        foreach (LevelDataSO level in GameManager.Instance.allLevels)
        {
            LevelCard data = GameManager.Instance.ModelDataLevelCard(level);
            LevelPrefab cardPrefab = Instantiate(levelPrefab, contentHolder);

            cardPrefab.mapNameTxt.text = data.levelName;
            cardPrefab.mapImg.sprite = data.mapIcon;
            cardPrefab.bestTimeTxt.text = Utility.ConvertTime(data.bestTime);
            cardPrefab.goldTimeTxt.text = data.goldTimeTxt;
            cardPrefab.silverTimeTxt.text = data.silverTimeTxt;
            cardPrefab.bronzeTimeTxt.text = data.bronzeTimeTxt;
            cardPrefab.medalImg.enabled = data.isThereBestTime;
            cardPrefab.medalImg.sprite = data.medalSprite;
            cardPrefab.playButton.interactable = data.isLevelUnlocked;

            cardPrefab.playButton.onClick.AddListener(() => {
                GameManager.Instance.currentLevel = level;
                LoadingScreen.Instance.SwitchToLevel(level.buildIndex, GameState.Playing);
            });
        }
    }

    // ------------------------------------------------------------------- //
    void SetUpSkinToggleList()
    {
        foreach (SkinDataSO skin in GameManager.Instance.allSkins)
        {
            SkinToggle skinDataModel = GameManager.Instance.ModelDataSkinToggle(skin);
            SkinTogglePrefab toggleSkin = Instantiate(togglePrefab, contentHolderToggles);
            toggleSkin.toggleSkinName.text = skinDataModel.skinName;
            toggleSkin.toggle.interactable = skinDataModel.isSkinUnlocked;
            toggleSkin.toggle.isOn = skinDataModel.selectedSkin;
            toggleSkin.toggle.group = toggleGroup;

            if (skinDataModel.isSkinUnlocked)
            {
                toggleSkin.toggle.onValueChanged.AddListener((isOn) => {
                    
                    if(isOn)
                    {
                        toggleSkin.toggle.interactable = false;
                        GameManager.Instance.SetCurrentSkin(skinDataModel.skinID);             
                    } else
                    {
                        toggleSkin.toggle.interactable = true;
                    }
            
                });
            }
        }
    }
  
}
