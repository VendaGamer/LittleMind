using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerController : MonoBehaviour, IInteractor
{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform pickupPoint;
    public Transform PickupPoint => pickupPoint;
    
    [Header("Interaction Settings")]
    [SerializeField] private GlobalInteractions globalInteractionsPlayerControls;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private float rayCastDistance = 3f;
    [CanBeNull] private IInteractable interactableLookingAt;
    [CanBeNull] public IInteractable InteractableHolding { get; private set; }

    [SerializeField]
    private float pickupLerpDuration = 1f;
    public float PickupLerpDuration => pickupLerpDuration;

    [SerializeField]
    private Interactions uiData;

    private void SwitchGlobalInteractions(GlobalInteractions newGlobalInteractions)
    {
        //uiData.UpdateInteractions(newGlobalInteractions, interactableLookingAt);
    }

    private void SwitchExclusiveInteractions(IInteractable newExclusiveInteractions)
    {
        //uiData.UpdateInteractions(globalInteractionsPlayerControls, newExclusiveInteractions);
    }

    private void HandleInteraction()
    {
        if (Physics.Raycast(
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0)), out var raycastHit,
                rayCastDistance, interactableLayerMask))
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
    
    private void HandleInteractableHit(IInteractable interactable)
    {
        // Only update if we're looking at a different interactable
        if (interactableLookingAt == interactable)
            return;

        if (interactableLookingAt != null)
        {
            interactableLookingAt.ToggleOutline(false);
                    
            if (interactableLookingAt.CurrentInteractions != interactable.CurrentInteractions)
            {
                SwitchExclusiveInteractions(interactable);
            }
            // Set new interactable
        }
        else
        {
            SwitchExclusiveInteractions(interactable);
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
        SwitchExclusiveInteractions(null);
    }
    
}
