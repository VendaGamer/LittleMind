using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public enum InputType
{
    KeyboardMouse,
    Gamepad
}

public struct InputDisplay
{
    public string Text { get; set; }        // Fallback text (e.g., "E", "Q")
    public string IconPath { get; set; }     // Path to icon asset if needed
    public InputType InputType { get; set; }
}
public interface IInteractable
{
    Interaction[] Interactions { get;}
    bool Interact(PlayerController interactor, InputAction invokedAction);
    [SerializeField] string InteractText { get; }
}

public struct Interaction
{
    public InputDisplay Hint;
    public InputAction InputAction;
}
