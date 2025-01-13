using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Interaction
{
    public string ActionName;
    public InputActionReference Action;
}
public interface IInteractable
{
    string InteractText { get; }
    Interaction[] Interactions { get; }
    bool Interact(IInteractor interactor, InputAction invokedAction);
}
public interface IInteractor
{
    Transform PickupPoint { get; }
    float PickupLerpDuration { get; }
}
public enum InputType
{
    KeyboardMouse,
    Gamepad
}
