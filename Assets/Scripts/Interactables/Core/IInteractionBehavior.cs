using UnityEngine.InputSystem;

public interface IInteractionBehavior
{
    bool CanExecute(IInteractor interactor, InputAction invokedAction);
    bool Execute(IInteractor interactor, InputAction invokedAction);
    void Initialize(Interactable interactable);
}