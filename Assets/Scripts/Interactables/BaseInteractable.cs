using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    protected abstract InteractableData InteractableData { get; }
    public string InteractGroupLabel => InteractableData.InteractableGroupLabel;
    public abstract Interaction[] CurrentInteractions { get; }
    public event Action InteractionsChanged;
    
    [CanBeNull]
    protected Outline Outline;

    protected virtual void Start()
    {
        Outline = GetComponent<Outline>();
    }
    
    protected void OnInteractionsChanged()
    {
        InteractionsChanged?.Invoke();
    }

    public abstract bool Interact(IInteractor interactor, InputAction invokedAction);

    public virtual void OnEndedLookingAt(IInteractor interactor)
    {
        if(Outline)
            Outline.enabled = false;
    }

    public virtual void OnStartedLookingAt(IInteractor interactor)
    {
        if(Outline)
            Outline.enabled = true;
    }
}