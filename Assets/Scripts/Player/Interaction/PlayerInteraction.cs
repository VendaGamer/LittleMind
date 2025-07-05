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
    private LayerMask interactableLayerMask;

    [SerializeField]
    private float rayCastDistance = 3f;

    [CanBeNull]
    private IInteractable interactableLookingAt;

    [CanBeNull]
    private IInteractable interactableHolding;
    
    public IInteractable InteractableHolding => interactableHolding;

    [SerializeField]
    private float pickupLerpDuration = 1f;
    public float PickupLerpDuration => pickupLerpDuration;

    private void HandleInteraction()
    {
        if (
            Physics.Raycast(
                PlayerCamera.Instance.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)),
                out var raycastHit,
                rayCastDistance,
                interactableLayerMask
            )
        )
        {
            if (raycastHit.collider.TryGetComponent<Interactable>(out var interactable))
            {
                // hit interactable, maybe the same, maybe new one
                HandleInteractableHit(interactable);
                return;
            }
        }
        
        ClearCurrentInteractable();
    }

    private void OnDrop(InputAction.CallbackContext obj)
    {
        if (InteractableHolding == null)
            return;

        if (InteractableHolding.Interact(this, obj.action))
        {
            interactableHolding = null;
            InteractionHandler.Instance.SetCurrentInteractable(null);
            OnInteractableDrop();
        }
    }

    private void HandleInteractableHit(Interactable interactable)
    {
        // Only update if we're looking at a different interactable
        if (ReferenceEquals(interactable, interactableLookingAt))
            return;
        
        interactableLookingAt?.OnEndedLookingAt(this);
        interactableLookingAt = interactable;
        interactable.OnStartedLookingAt(this);
        InteractionHandler.Instance.SetCurrentInteractable(interactableLookingAt);
    }
    
    private void ClearCurrentInteractable()
    {
        if (interactableLookingAt == null)
            return;

        interactableLookingAt.OnEndedLookingAt(this);
        interactableLookingAt = null;

        InteractionHandler.Instance.SetCurrentInteractable(interactableHolding);
    }
    
    private void OnUse(InputAction.CallbackContext obj)
    {
        if (interactableHolding == null)
        {
            interactableLookingAt?.Interact(this, obj.action);
        }
        else
        {
            interactableHolding.Interact(this, obj.action);
        }
    }
    
    public void PickUp(IInteractable itemToPickUp)
    {
        interactableHolding = itemToPickUp;
        InteractionHandler.Instance.SetCurrentInteractable(interactableHolding);
    }
    
}
