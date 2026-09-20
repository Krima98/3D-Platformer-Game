using UnityEngine;
using System;

public class RunTimer : MonoBehaviour
{    
    public static event Action<float> onLevelFinish;
    public static event Action<float> onTimerCount;

    private float currentTime;
    private bool isTimerRunning;

    void Update()
    {
        CountTimer();
    }

    // -------------------------------------------- //
    void CountTimer()
    {
        if(isTimerRunning)
        {
            currentTime += Time.deltaTime;
            onTimerCount?.Invoke(currentTime);
        }
    }

    // -------------------------------------------- //
    public void StartTimer() 
    {
        isTimerRunning = true;
        currentTime = 0;
    }

    // -------------------------------------------- //
    public void StopTimer()
    {
        isTimerRunning = false;
        onLevelFinish?.Invoke(currentTime);
        GameManager.Instance.ChangeGameState(GameState.LevelComplete);
    }
}
