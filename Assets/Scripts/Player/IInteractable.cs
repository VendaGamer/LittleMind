using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Interaction
{
    public string ActionName;
    public InputActionReference Action;
}
public interface IInteractable : IInteractions
{
    public bool Interact(IInteractor interactor, InputAction invokedAction);
}

[Serializable]
public class GlobalInteractions : IInteractions
{
    [SerializeField] private string interactGroupLabel;
    [SerializeField] private Interaction[] currentInteractions;
    public string InteractGroupLabel => interactGroupLabel;
    public Interaction[] CurrentInteractions => currentInteractions;

}

public interface IInteractions
{
    public string InteractGroupLabel { get;}
    public Interaction[] CurrentInteractions { get;}
}
public interface IInteractor
{
    public Transform PickupPoint { get; }
    public float PickupLerpDuration { get; }
}
public enum InputType
{
    KeyboardMouse,
    Gamepad
}