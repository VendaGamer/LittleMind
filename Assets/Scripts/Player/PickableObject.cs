using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactableLabel;
    public string InteractGroupLabel => interactableLabel;
    protected bool IsPicked = false;
    [SerializeField] private Interaction pickupAction;
    [SerializeField] private Interaction dropAction;

    private Rigidbody rb;
    private Collider col;

    public Interaction[] CurrentInteractions
    {
        get
        {
            if (IsPicked)
            {
                return new[] {  dropAction};
            }
            return new[] { pickupAction };
        }
    }

    protected virtual void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    public void DropObject()
    {
        StopAllCoroutines();
        rb.isKinematic = false;
        col.isTrigger = false;
        transform.SetParent(null);
        OnDropped();
    }
    public void PickObject(Transform parent,float pickDur)
    {
        rb.isKinematic = true;
        col.isTrigger = true;
        StartCoroutine(PerformPickupLerp(parent, pickDur));
    }

    private IEnumerator PerformPickupLerp(Transform pickupPoint, float pickDur)
    {
        transform.SetParent(pickupPoint);
        var endPos = Vector3.zero;
        var endRot = Quaternion.Euler(Vector3.zero);

        transform.GetLocalPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        float elapsedTime = 0f;
        while (elapsedTime < pickDur)
        {
            elapsedTime += Time.deltaTime;
            var step = Mathf.SmoothStep(0, 1, elapsedTime / pickDur);

            transform.SetLocalPositionAndRotation(
                Vector3.Lerp(startPos, endPos, step),
                Quaternion.Lerp(startRot, endRot, step)
            );
            yield return null;
        }
        
        transform.SetLocalPositionAndRotation(
            endPos,
            endRot
        );
        OnPicked();
    }


    protected virtual void OnPicked() { }
    protected virtual void OnDropped() { }
    
    public bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsPicked)
        {
            if (invokedAction.id == dropAction.Action.action.id)
            {
                Debug.Log("Dropped Object");
                DropObject();
                IsPicked = false;
                return true;
            }
        }
        else if (invokedAction.id == pickupAction.Action.action.id)
        {
            Debug.Log("Picked Object");
            PickObject(interactor.PickupPoint,interactor.PickupLerpDuration);
            IsPicked = true;
            return true;
        }
        return false;
    }
    
}
