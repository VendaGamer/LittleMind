using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    protected abstract InteractableData InteractableData { get; }
    public string InteractGroupLabel => InteractableData.InteractableGroupLabel;
    public abstract Interaction[] CurrentInteractions { get; }
    public event Action InteractionsChanged;
    
    public abstract bool Interact(IInteractor interactor, InputAction invokedAction);

    protected void OnInteractionsChanged()
    {
        InteractionsChanged?.Invoke();
    }

    public virtual void OnEndedToLookAt(IInteractor interactor)
    {
        
    }

    public virtual void OnStartedToLookAt(IInteractor interactor)
    {
        
    }
}