using System.Collections.Generic;

[System.Serializable]
public class GameData
{

    public bool turnOffRotation;
    public int currentSkinID;
    public List<int> unlockedSkins; 
    public SoundSettings soundSettings;
    public MouseSensitivity mouseSensitivity;
    public List<LevelData> level;

    public GameData()
    {
        this.turnOffRotation = false;        
        this.soundSettings = new SoundSettings();
        this.mouseSensitivity = new MouseSensitivity();
        this.level = new List<LevelData>();
        this.unlockedSkins = new List<int> {0}; // 
        this.currentSkinID = 0;
        this.level.Add(new LevelData(1, true));
    }

    public LevelData FindLevelData(int id)
    {
        foreach (LevelData levelData in level)
        {
            if (levelData.levelID == id)
            {
                return levelData;
            }
        }
        return null; 
    }
}

// --------------------------- //
[System.Serializable]
public class SoundSettings 
{
    public float masterVolume;
    public float uiVolume;
    public float musicVolume;
    public float sfxVolume;

    public SoundSettings()
    {
        this.masterVolume = 0.5f;
        this.uiVolume = 1f;
        this.musicVolume = 1f;
        this.sfxVolume = 1f;
    }
}

// --------------------------- //
[System.Serializable]
public class MouseSensitivity
{
    public float sensitivity;

    public MouseSensitivity()
    {
        this.sensitivity = 1f;
    }
}

// --------------------------- //

[System.Serializable]
public class LevelData
{
    public int levelID;
    public float bestTime;
    public bool mapUnlcoked;

    public LevelData(int id, bool unlocked)
    {
        this.levelID = id;
        this.mapUnlcoked = unlocked;
        this.bestTime = 0f;
    }
}



