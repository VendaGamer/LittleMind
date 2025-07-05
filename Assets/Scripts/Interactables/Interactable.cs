using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using ZLinq;

[RequireComponent(typeof(IInteractionProvider))]
public sealed class Interactable : MonoBehaviour, IInteractable
{
    public string InteractGroupLabel => "";
    public IReadOnlyList<Interaction> CurrentInteractions => InteractionProvider.CurrentInteractions;

    public event Action InteractionsChanged
    {
        add => InteractionProvider.InteractionsChanged += value;
        remove => InteractionProvider.InteractionsChanged -= value;
    }

    [CanBeNull]
    private Outline Outline;

    [NotNull]
    public IInteractionProvider InteractionProvider { get; private set; } = null!;

    private void Start()
    {
        Outline = GetComponent<Outline>();
        InteractionProvider = GetComponent<IInteractionProvider>();
    }

    public void ChangeState(StatefulInteractionProvider.InteractionState state)
    {
        
    }

    public bool Interact(IInteractor interactor, InputAction invokedAction) =>
        InteractionProvider.CurrentInteractions.AsValueEnumerable()
            .First(i => ReferenceEquals(i.Action, invokedAction)) is not null;

    public void OnEndedLookingAt(IInteractor interactor)
    {
        if(Outline)
            Outline.enabled = false;
    }

    public void OnStartedLookingAt(IInteractor interactor)
    {
        if(Outline)
            Outline.enabled = true;
    }
}