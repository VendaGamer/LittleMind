using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected PickableObjectInfo info;
    protected bool IsPicked = false;
    private CancellationTokenSource curTokScr;
    public string InteractGroupLabel => info.InteractableGroupLabel;

    private Rigidbody rb;
    private Collider col;
    private Outline outline;
    private Coroutine currentPickupCoroutine;

    public Interaction[] CurrentInteractions
    {
        get
        {
            return IsPicked ? new[] {  info.DropInteraction } : new[] { info.PickupInteraction };
        }
    }

    protected virtual void Start()
    {
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
        transform.parent.SetParent(null);
        OnDropped();
    }
    private void PickObject(Transform parent)
    {
        rb.isKinematic = true;
        col.isTrigger = true;
        transform.parent.SetParent(parent);
    
        if (currentPickupCoroutine != null)
            StopCoroutine(currentPickupCoroutine);
        
        currentPickupCoroutine = StartCoroutine(PerformPickupLerp());
    }

    private IEnumerator PerformPickupLerp()
    {
        var endPos = Vector3.zero;
        var endRot = Quaternion.Euler(Vector3.zero);

        transform.parent.GetLocalPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        float elapsedTime = 0f;
    
        while (elapsedTime < info.LerpDuration)
        {
            elapsedTime += Time.deltaTime;
            var step = Mathf.SmoothStep(0, 1, elapsedTime / info.LerpDuration);

            transform.parent.SetLocalPositionAndRotation(
                Vector3.Lerp(startPos, endPos, step),
                Quaternion.Lerp(startRot, endRot, step)
            );
        
            yield return null;
        }
    
        transform.parent.SetLocalPositionAndRotation(endPos, endRot);
        OnPicked();
    }


    protected virtual void OnPicked() { }
    protected virtual void OnDropped() { }
    
    public virtual bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsPicked)
        {
            if (invokedAction.id == info.DropInteraction.ActionRef.action.id)
            {
                DropObject();
                IsPicked = false;
                return true;
            }
        }
        else if (invokedAction.id == info.PickupInteraction.ActionRef.action.id)
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
