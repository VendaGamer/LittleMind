using System;
using UnityEngine.InputSystem;

public interface IInteractable : IInteractionGroup
{
    public bool Interact(IInteractor interactor, InputAction invokedAction);
    public bool ToggleOutline(bool value);

    public event Action InteractionsChanged;
}