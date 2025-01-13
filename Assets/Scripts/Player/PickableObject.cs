using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickableObject : MonoBehaviour, IInteractable
{
    protected bool IsPicked = false;
    [SerializeField] private InputActionReference pickupAction;
    [SerializeField] private InputActionReference dropAction;

    private Rigidbody rb;
    private Collider col;
    
    public string InteractText { get; }
    public Interaction[] Interactions { get; }
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
    
    public bool Interact(PlayerController interactor, InputAction invokedAction)
    {
        if (IsPicked && invokedAction == pickupAction.action)
        {
            DropObject();
            return true;
        }
        
        if (invokedAction == dropAction.action)
        {
            PickObject(interactor.PickupPoint,interactor.PickupLerpDuration);
            return true;
        }

        return false;
    }
    
}
