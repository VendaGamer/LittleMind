using UnityEngine;

public interface IInteractor
{
    public Transform PickupPoint { get; }
    public void PickUp(Interactable itemToPickUp);
    public Interactable InteractableHolding { get; }

    public void SetIKTarget(IKTargetType targetType);
}