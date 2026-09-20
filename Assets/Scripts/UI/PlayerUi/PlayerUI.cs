using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Fast run particle")]
    [SerializeField] ParticleSystem particleExtraSpeed;

    [Header("UI container")]
    [SerializeField] GameObject UIContainer;

    [Header("Stamina")]
    [SerializeField] private Image staminaFill;
    [SerializeField] private float fillStamina;

    [Header("ExtraMovementSpeed")]
    [SerializeField] private Image speedbarFill;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private float smoothFillSpeed;

    [Header("SpeedEffect")]
    [SerializeField] private GameObject speedEffect;

    [Header("Timer Txt")]
    [SerializeField] TMP_Text liveTimeTxt;

    // ------------------------------------------------------------------- //
    void OnEnable()
    {
        RunTimer.onTimerCount += UpdateTimeLive;
        GameStateMachine.OnStateEnter += EnterState;
    }

    // ------------------------------------------------------------------- //
    void OnDisable()
    {
        RunTimer.onTimerCount -= UpdateTimeLive;
        GameStateMachine.OnStateEnter -= EnterState;
    }

    // ------------------------------------------------------------------- //
    void Update()
    {
        SpeedScreenEffect();
        UpdateBar(playerScript.currentStamina, playerScript.maxStamina, ref fillStamina, staminaFill);
        UpdateBar(playerScript.extraSpeed, playerScript.maxExtraSpeed, ref smoothFillSpeed, speedbarFill);
    }

    // ------------------------------------------------------------------- //
    void SpeedScreenEffect()
    {
        if(playerScript.extraSpeed > 0f)
        {
            particleExtraSpeed.Play();
            
        } else
        {
            particleExtraSpeed.Stop();
        }
    }

    // ------------------------------------------------------------------- //
    private void UpdateBar(float current, float max, ref float smoothValue, Image fillImage)
    {
        float target = current / max;
        smoothValue = Mathf.Lerp(smoothValue, target, Time.deltaTime * 8f);
        fillImage.fillAmount = smoothValue;
    }

    //======= EVENTS ========//
    private void UpdateTimeLive(float time)
    {
        liveTimeTxt.text = Utility.ConvertTime(time);
    }

    // ------------------------------------------------------------------- //
    private void EnterState(GameState state)
    {
        switch(state)
        {
            case GameState.LevelComplete:
                UIContainer.SetActive(false);
                break;
        }
    }
}

