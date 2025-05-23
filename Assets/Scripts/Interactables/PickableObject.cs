using System.Collections;
using System.Threading;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PickableObject : MonoBehaviour, IInteractable
{
    protected abstract PickableObjectData Data { get; }
    protected bool IsPicked = false;
    private CancellationTokenSource curTokScr;
    public string InteractGroupLabel => Data.InteractableGroupLabel;

    private Rigidbody rb;
    private Collider col;
    private Outline outline;
    private Coroutine currentPickupCoroutine;
    private Transform Container;

    [CreateProperty]
    public Interaction[] CurrentInteractions
    {
        get
        {
            return IsPicked ? new[] {  Data.DropInteraction } : new[] { Data.PickupInteraction };
        }
    }

    protected virtual void Start()
    {
        Container = transform.parent ?? transform;
        rb = GetComponentInParent<Rigidbody>();
        col = GetComponent<Collider>();
        outline = GetComponent<Outline>();
    }
    private void DropObject()
    {
        if (currentPickupCoroutine != null)
            StopCoroutine(currentPickupCoroutine);
        
        rb.isKinematic = false;
        col.isTrigger = false;
        Container.SetParent(null);
        OnDropped();
    }
    private void PickObject(Transform parent)
    {
        rb.isKinematic = true;
        col.isTrigger = true;
        Container.SetParent(parent);
    
        if (currentPickupCoroutine != null)
            StopCoroutine(currentPickupCoroutine);
        
        currentPickupCoroutine = StartCoroutine(PerformPickupLerp());
    }

    private IEnumerator PerformPickupLerp()
    {
        var endPos = Vector3.zero;
        var endRot = Quaternion.Euler(Vector3.zero);

        Container.GetLocalPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        float elapsedTime = 0f;
    
        while (elapsedTime < Data.LerpDuration)
        {
            elapsedTime += Time.deltaTime;
            var step = Mathf.SmoothStep(0, 1, elapsedTime / Data.LerpDuration);

            Container.SetLocalPositionAndRotation(
                Vector3.Lerp(startPos, endPos, step),
                Quaternion.Lerp(startRot, endRot, step)
            );
        
            yield return null;
        }
    
        Container.SetLocalPositionAndRotation(endPos, endRot);
        OnPicked();
    }


    protected virtual void OnPicked() { }
    protected virtual void OnDropped() { }
    
    public virtual bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsPicked)
        {
            if (invokedAction.id == Data.DropInteraction.ActionRef.action.id)
            {
                DropObject();
                IsPicked = false;
                return true;
            }
        }
        else if (invokedAction.id == Data.PickupInteraction.ActionRef.action.id)
        {
            PickObject(interactor.PickupPoint);
            IsPicked = true;
            interactor.PickUp(this);
            return true;
        }
        return false;
    }

    public bool ToggleOutline(bool value)
    {
        outline.enabled = value;
        return true;
    }
}
