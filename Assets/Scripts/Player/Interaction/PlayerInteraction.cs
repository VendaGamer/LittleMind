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
    private IInteractable _interactableHolding;

    [CanBeNull]
    public IInteractable InteractableHolding
    {
        get=> _interactableHolding;
        private set
        {
            if(ReferenceEquals(_interactableHolding, value))
                return;
            
            _interactableHolding = value;
            interactionHandler.SetCurrentInteractableInteractions(value);
        }
    }

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
    
    public void PickUp(IInteractable itemToPickUp)
    {
        InteractableHolding = itemToPickUp;

    }
    
    private void OnDrop(InputAction.CallbackContext obj)
    {
        if (InteractableHolding == null)
            return;

        if (InteractableHolding.Interact(this, obj.action))
        {
            InteractableHolding = null;
        }
    }

    private void HandleInteractableHit(IInteractable interactable)
    {
        // Only update if we're looking at a different interactable
        if (ReferenceEquals(interactable, interactableLookingAt))
            return;

        if (interactableLookingAt != null)
        {
            interactableLookingAt.ToggleOutline(false);

            if (interactableLookingAt.CurrentInteractions != interactable.CurrentInteractions)
            {
                interactionHandler.SetCurrentInteractableInteractions(interactableLookingAt);
            }
            // Set new interactable
        }

        interactable.ToggleOutline(true);
        interactableLookingAt = interactable;
    }

    private void ClearCurrentInteractable()
    {
        if (interactableLookingAt == null)
            return;

        interactableLookingAt.ToggleOutline(false);
        interactableLookingAt = null;
    }
}
