using System;
using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
[GeneratePropertyBag]
public abstract partial class Interactable : MonoBehaviour, IInteractionGroup
{
    protected abstract InteractableData InteractableData { get; }
    public string InteractGroupLabel => InteractableData.InteractableGroupLabel;
    public abstract Interaction[] CurrentInteractions { get; }
    protected abstract ReadOnlyArray<Interaction> AllInteractions { get; }
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
        if(Outline is not null)
            Outline.enabled = false;
    }

    public virtual void OnStartedLookingAt(IInteractor interactor)
    {
        if(Outline is not null)
            Outline.enabled = true;
    }

    public void RebuildKeys()
    {
        for (var i = 0; i < AllInteractions.Count; i++)
        {
            AllInteractions[i].RebuildKey();
        }
    }
}