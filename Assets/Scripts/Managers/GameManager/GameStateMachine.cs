using System;
using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    [SerializeField] public GameState currentState;
    
    public static event Action<GameState> OnStateEnter;
    public static event Action<GameState> OnStateExit;

    public void SetState(GameState newState)
    {
        if (currentState == newState)
        {
            return;
        } else
        {
            OnStateExit?.Invoke(currentState);
            currentState = newState;
            OnStateEnter?.Invoke(newState);
        }
    }
}
