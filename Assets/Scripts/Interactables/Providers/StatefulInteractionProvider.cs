using System;
using System.Collections.Generic;
using UnityEngine;
using ZLinq;

public class StatefulInteractionProvider : MonoBehaviour, IInteractionProvider
{
    [Tooltip("The list of all possible states and their corresponding interactions.")]
    [SerializeField] private InteractionState[] allStates;
    
    [SerializeField] private Interaction[] allInteractions;
    
    [Tooltip("The name of the state this object should start in.")]
    [SerializeField] private int initialStateIndex;
    
    private InteractionState _currentState;

    public event Action InteractionsChanged;

    private void Awake()
    {
        ChangeState(initialStateIndex);
    }

    public IReadOnlyList<Interaction> CurrentInteractions =>
        _currentState.InteractionsIndexes.AsValueEnumerable().Select(i => allInteractions[i]).ToArray();

    /// <summary>
    /// Changes the provider to a new state, updating the available interactions.
    /// This should be called by an InteractionResponse.
    /// </summary>
    public void ChangeState(int index)
    {
        var newState = allStates[index];
        if (ReferenceEquals(_currentState, newState)) return;
        

        _currentState = newState;
        Debug.Log($"Interactable '{newState.StateName}' changing state to '{newState.StateName}'");
        
        InteractionsChanged?.Invoke();
    }
    
    [Serializable]
    public class InteractionState
    {
        public string StateName;
        public int[] InteractionsIndexes;
    }
}

