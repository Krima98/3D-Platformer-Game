using UnityEngine;

public struct LevelComplete
{
    public string levelName;
    public float currentRunTime;
    public float bestTime;
    public Sprite medalSprite;
    public Sprite rewardSprite;
    public bool isNewRecord;
    public bool isNextLevelOpen;
    public LevelDataSO nextLevel;
    public string goldTimeTxt;
    public string silverTimeTxt;
}

public struct LevelCard
{
    public string levelName;
    public float bestTime;
    public Sprite mapIcon;
    public Sprite medalSprite;
    public string goldTimeTxt;
    public string silverTimeTxt;
    public string bronzeTimeTxt;
    public LevelDataSO selectedLevelSO;
    public bool isLevelUnlocked;
    public bool isThereBestTime;
}

public struct SkinToggle
{
    public string skinName;
    public bool isSkinUnlocked;
    public bool selectedSkin;
    public int skinID;
    public GameData data;
}

