using System;
using System.IO;
using UnityEngine;

public class SaveReadManager : MonoBehaviour
{
    public static SaveReadManager Instance;

    [Header("FileLocation for Jsonfile")]
    public string filename;
    private string filePath;
    public GameData gameData;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, filename);
        } else
        {
            Destroy(gameObject);    
        }
    }

    // --------------------------- //
    void Start()
    {
        LoadData();
    }

    // --------------------------- //
    public void SaveData()
    {
        try
        {
            string jsonDataToStore = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(filePath, jsonDataToStore);
        } catch (Exception)
        {
            // her kunne det blitt lagt til feilmelding for bruker i from av å sende event til UI element
        }
    }

    // --------------------------- //
    void LoadData()
    {
        if(File.Exists(filePath))
        {
            try
            {
                string jsonGameData = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<GameData>(jsonGameData);
            } catch
            {
                
            }
        } else
        {
            gameData = new GameData();
        }

        FillAllLevlDataToJson();
        SaveData();
    }

    // --------------------------- //
    private void OnApplicationQuit()
    {
        SaveData();    
    }

    // --------------------------- //
    private void FillAllLevlDataToJson()
    {
        LevelDataSO[] levelList = GameManager.Instance.allLevels;

        foreach (LevelDataSO level in levelList)
        {
            LevelData existingData = gameData.FindLevelData(level.levelID);

            if (existingData == null)
            {
                bool defaultUnlock = level.levelID == 1;
                gameData.level.Add(new LevelData(level.levelID, defaultUnlock));
            }
        } 
    } 

}
