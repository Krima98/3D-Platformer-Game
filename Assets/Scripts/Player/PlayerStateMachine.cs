using System;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public StatePlayer currentState;
    
    public static event Action<StatePlayer> OnStateEnter;
    public static event Action<StatePlayer> OnStateExit;

    public void SetState(StatePlayer newState)
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
