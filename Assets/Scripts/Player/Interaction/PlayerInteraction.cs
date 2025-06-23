using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform pickupPoint;
    public Transform PickupPoint => pickupPoint;

    [Header("Interaction Settings")]
    [SerializeField]
    private GlobalInteractionGroup globalInteractionGroupPlayerControls;

    [SerializeField]
    private InteractionHandler interactionHandler;

    [SerializeField]
    private LayerMask interactableLayerMask;

    [SerializeField]
    private float rayCastDistance = 3f;

    [CanBeNull]
    private IInteractable interactableLookingAt;

    [CanBeNull]
    private IInteractable interactableHolding;
    
    public void PickUp(IInteractable itemToPickUp) => interactableHolding = itemToPickUp;
    public IInteractable InteractableHolding => interactableHolding;

    [SerializeField]
    private float pickupLerpDuration = 1f;
    public float PickupLerpDuration => pickupLerpDuration;

    private void HandleInteraction()
    {
        if (
            Physics.Raycast(
                playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)),
                out var raycastHit,
                rayCastDistance,
                interactableLayerMask
            )
        )
        {
            if (raycastHit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                // hit interactable, maybe the same, maybe new one
                HandleInteractableHit(interactable);
            }
            else
            {
                // hit something that is not interactable
                ClearCurrentInteractable();
            }
        }
        else
        {
            // hit nothing
            ClearCurrentInteractable();
        }
    }

    private void OnDrop(InputAction.CallbackContext obj)
    {
        if (InteractableHolding == null)
            return;

        if (InteractableHolding.Interact(this, obj.action))
        {
            interactableHolding = null;
            OnInteractableDrop();
        }
    }

    private void HandleInteractableHit(IInteractable interactable)
    {
        // Only update if we're looking at a different interactable
        if (ReferenceEquals(interactable, interactableLookingAt))
            return;
        
        interactableLookingAt?.OnEndedToLookAt(this);
        interactableLookingAt = interactable;
        interactable.OnStartedToLookAt(this);
        interactionHandler.SetCurrentInteractable(interactable);
    }

    private void ClearCurrentInteractable()
    {
        if (interactableLookingAt == null)
            return;

        interactableLookingAt.OnEndedToLookAt(this);
        interactableLookingAt = null;
        interactionHandler.SetCurrentInteractable(null);
    }
    
    private void OnUse(InputAction.CallbackContext obj)
    {
        interactableLookingAt?.Interact(this, obj.action);
    }
    
}
