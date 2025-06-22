using UnityEngine;

public interface IInteractor
{
    public Transform PickupPoint { get; }
    public void PickUp(IInteractable itemToPickUp);
    public IInteractable InteractableHolding { get; }

    public void SetHandTarget(IKTargetType targetType);
}