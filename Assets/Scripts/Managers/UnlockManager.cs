using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    private void OnEnable()
    {
        MapCompleteView.OnUIsetup += UnlockCheck;
    }

    // --------------------------- //
    private void OnDisable()
    {
        MapCompleteView.OnUIsetup -= UnlockCheck;
    }

    // --------------------------- //
    void UnlockCheck(float runTime)
    {
        GameData gameData = SaveReadManager.Instance.gameData;
        LevelDataSO currentLevel = GameManager.Instance.currentLevel;
        
        CheckRecord(gameData, currentLevel, runTime);
        CheckSkinUnlock(gameData, currentLevel, runTime);

        int nextLevelID = currentLevel.levelID + 1;
        if(nextLevelID <= GameManager.Instance.allLevels.Length)
        {
            CheckNextMapUnlock(gameData, nextLevelID, currentLevel, runTime);
        } 

        SaveReadManager.Instance.SaveData(); 
    }

    // ---------------------------------------------------------------------------- //

    void CheckNextMapUnlock(GameData gameData, int nextLevelID, LevelDataSO currentLevel, float runTime)
    {
        LevelData levelData = gameData.FindLevelData(nextLevelID);

        if (runTime <= currentLevel.silverTime)
        {
            levelData.mapUnlcoked = true;
        } 
    }

    // ---------------------------------------------------------------------------- //

    void CheckRecord(GameData gameData, LevelDataSO currentLevel, float runTime)
    {
        LevelData levelData = gameData.FindLevelData(currentLevel.levelID);

        if(runTime < levelData.bestTime || levelData.bestTime == 0)
        {
            levelData.bestTime = runTime;
        }
    }

    // ---------------------------------------------------------------------------- //
    
    void CheckSkinUnlock(GameData gameData, LevelDataSO currentLevel, float runTime)
    {
        bool isSkinUnlcoked = gameData.unlockedSkins.Contains(currentLevel.skinSO.skinID);

        if(runTime <= currentLevel.goldTime && !isSkinUnlcoked)
        {
            gameData.unlockedSkins.Add(currentLevel.skinSO.skinID);
        }
    }
}
