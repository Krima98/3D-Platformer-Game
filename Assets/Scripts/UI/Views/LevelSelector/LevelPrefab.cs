using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelPrefab : MonoBehaviour
{
    [Header("Media")]
    public Button playButton;
    public Image mapImg;
    public Image medalImg;

    [Header("Txt")]
    public TextMeshProUGUI bestTimeTxt;
    public TextMeshProUGUI mapNameTxt;
    public TextMeshProUGUI goldTimeTxt;
    public TextMeshProUGUI silverTimeTxt;
    public TextMeshProUGUI bronzeTimeTxt;
}
