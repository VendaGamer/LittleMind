using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[Serializable]
public class Interaction
{
    public string ActionName;
    [FormerlySerializedAs("Action")] public InputActionReference ActionRef;
}
public interface IInteractable : IInteractionGroup
{
    public bool Interact(IInteractor interactor, InputAction invokedAction);
    public bool ToggleOutline(bool value);
}

[Serializable]
public class GlobalInteractionGroup : IInteractionGroup
{
    [SerializeField] private string interactGroupLabel;
    [SerializeField] private Interaction[] currentInteractions;
    public string InteractGroupLabel => interactGroupLabel;
    public Interaction[] CurrentInteractions => currentInteractions;

}

public interface IInteractionGroup
{
    public string InteractGroupLabel { get;}
    public Interaction[] CurrentInteractions { get;}
}
public interface IInteractor
{
    public Transform PickupPoint { get; }
    public void PickUp(IInteractable itemToPickUp);
    public IInteractable InteractableHolding { get; }
}