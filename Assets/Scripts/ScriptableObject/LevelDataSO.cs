using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Map/LevelData")]
public class LevelDataSO : ScriptableObject
{
    [Header("MapData")]
    public int levelID;
    public string levelName;
    public int buildIndex;

    [Header("MapTimer")]
    public float goldTime;
    public float silverTime;

    [Header("img")]
    public Sprite mapIcon;

    [Header("MedalsIcon")]
    public Sprite goldIcon;
    public Sprite silverIcon;
    public Sprite bronzeIcon;

    [Header("Reward")]
    public SkinDataSO skinSO;
}




