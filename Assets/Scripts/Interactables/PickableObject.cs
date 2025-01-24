using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactableLabel;
    public string InteractGroupLabel => interactableLabel;
    protected bool IsPicked = false;
    [SerializeField] private Interaction pickupAction;
    [SerializeField] private Interaction dropAction;
    [CanBeNull] private Coroutine pickupCoroutine;

    private Rigidbody rb;
    private Collider col;

    public Interaction[] CurrentInteractions
    {
        get
        {
            return IsPicked ? new[] {  dropAction } : new[] { pickupAction };
        }
    }

    protected virtual void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    private void DropObject()
    {
        StopCoroutine(pickupCoroutine);
        rb.isKinematic = false;
        col.isTrigger = false;
        transform.parent.SetParent(null);
        OnDropped();
    }
    private void PickObject(Transform parent,float pickDur)
    {
        rb.isKinematic = true;
        col.isTrigger = true;
        transform.parent.SetParent(parent);
        pickupCoroutine = StartCoroutine(PerformPickupLerp(pickDur));
    }

    private IEnumerator PerformPickupLerp(float pickDur)
    {
        var endPos = Vector3.zero;
        var endRot = Quaternion.Euler(Vector3.zero);

        transform.parent.GetLocalPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        float elapsedTime = 0f;
        while (elapsedTime < pickDur)
        {
            elapsedTime += Time.deltaTime;
            var step = Mathf.SmoothStep(0, 1, elapsedTime / pickDur);

            transform.parent.SetLocalPositionAndRotation(
                Vector3.Lerp(startPos, endPos, step),
                Quaternion.Lerp(startRot, endRot, step)
            );
            yield return null;
        }
        
        transform.parent.SetLocalPositionAndRotation(
            endPos,
            endRot
        );
        OnPicked();
    }


    protected virtual void OnPicked() { }
    protected virtual void OnDropped() { }
    
    public virtual bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsPicked)
        {
            if (invokedAction.id == dropAction.Action.action.id)
            {
                DropObject();
                IsPicked = false;
                return true;
            }
        }
        else if (invokedAction.id == pickupAction.Action.action.id)
        {
            PickObject(interactor.PickupPoint,interactor.PickupLerpDuration);
            IsPicked = true;
            interactor.PickUp(this);
            return true;
        }
        return false;
    }
    
}
