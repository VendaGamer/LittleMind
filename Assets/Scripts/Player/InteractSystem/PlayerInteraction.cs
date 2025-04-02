using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerController : MonoBehaviour, IInteractor
{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform pickupPoint;
    public Transform PickupPoint => pickupPoint;

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
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.nearClipPlane));
        int hitCount = Physics.SphereCastNonAlloc(
            ray,
            sphereCastRadius,
            raycastHits,
            sphereCastDistance,
            interactableLayerMask
        );

        if (hitCount > 0)
        {
            RaycastHit hit = raycastHits[0];
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                // If we're looking at a different interactable, update interactions
                if (interactableLookingAt != interactable)
                {
                    if (!interactableLookingAt.IsUnityNull())
                    {
                        interactableLookingAt.ToggleOutline(false);
                        if (
                            interactableLookingAt.CurrentInteractions
                            != interactable.CurrentInteractions
                        )
                        {
                            SwitchExclusiveInteractions(interactable);
                        }
                    }

                    interactable.ToggleOutline(true);
                    interactableLookingAt = interactable;
                }
            }
            else
            {
                // We hit something, but it's not an interactable
                if (interactableLookingAt.IsUnityNull())
                    return;

                interactableLookingAt.ToggleOutline(false);
                interactableLookingAt = null;
                SwitchExclusiveInteractions(null);
            }
        }
        else
        {
            // We didn't hit anything
            if (!interactableLookingAt.IsUnityNull())
            {
                interactableLookingAt.ToggleOutline(false);
                interactableLookingAt = null;
                SwitchExclusiveInteractions(null);
            }
        }
    }
}
