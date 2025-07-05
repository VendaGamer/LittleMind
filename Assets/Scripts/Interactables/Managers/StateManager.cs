using System;
using UnityEngine;

[Serializable]
public class StateManager<T> where T : Enum
{
    [SerializeField] private T currentState;
    public T CurrentState => currentState;
    
    public event Action<T, T> StateChanged;
    
    public bool ChangeState(T newState)
    {
        if (currentState.Equals(newState)) return false;
        
        var previousState = currentState;
        currentState = newState;
        StateChanged?.Invoke(previousState, newState);
        return true;
    }
}