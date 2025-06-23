using System;
using UnityEngine.InputSystem;

public interface IInteractable : IInteractionGroup
{
    public bool Interact(IInteractor interactor, InputAction invokedAction);
    public void OnEndedToLookAt(IInteractor interactor);

    public void OnStartedToLookAt(IInteractor interactor);

    public event Action InteractionsChanged;
}