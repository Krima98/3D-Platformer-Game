using UnityEngine;

[CreateAssetMenu(menuName = "Skins/SkinData")]
public class SkinDataSO : ScriptableObject
{
    public int skinID; 
    public string skinName;

    [Header("Sprite")]
    public Sprite rewardSkin;
    public Sprite rewardOwned;

    [Header("Material")]
    public Material materialHead;
    public Material materialBody;
}
