using System;
using UnityEngine.InputSystem;

public interface IInteractable : IInteractionGroup
{
    public bool Interact(IInteractor interactor, InputAction invokedAction);
    public void OnEndedLookingAt(IInteractor interactor);

    public void OnStartedLookingAt(IInteractor interactor);

    public event Action InteractionsChanged;
}