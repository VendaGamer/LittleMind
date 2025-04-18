using System.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Door : MonoBehaviour, IInteractable
{
    [FormerlySerializedAs("info")] [SerializeField] protected DoorData data;
    private Quaternion closedRotation;
    protected bool IsOpen = false;
    public string InteractGroupLabel => data.InteractableGroupLabel;
    private Coroutine currentRotateCoroutine;
    
    private void Start()
    {
        closedRotation = transform.parent.rotation;
    }
    [CreateProperty]
    public virtual Interaction[] CurrentInteractions
    {
        get
        {
            return IsOpen ? new[] { data.CloseDoorInteraction } : new[] { data.OpenDoorInteraction, data.LookThroughKeyHoleInteraction };
        }
    }

    public virtual bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsOpen)
        {
            if (invokedAction.id == data.CloseDoorInteraction.ActionRef.action.id)
            {
                if (currentRotateCoroutine != null)
                    StopCoroutine(currentRotateCoroutine);
                    
                currentRotateCoroutine = StartCoroutine(RotateDoor(closedRotation));
                IsOpen = false;
                return true;
            }
        }
        else if(invokedAction.id == data.OpenDoorInteraction.ActionRef.action.id)
        {
            if (currentRotateCoroutine != null)
                StopCoroutine(currentRotateCoroutine);
                
            currentRotateCoroutine = StartCoroutine(RotateDoor(
                closedRotation * Quaternion.Euler(0f, data.OpenAngle, 0f)));
            IsOpen = true;
            return true;
        }
        return false;
    }

    public bool ToggleOutline(bool value)
    {
        return false;
    }

    private IEnumerator RotateDoor(Quaternion desiredRotation)
    {
        Quaternion startRotation = transform.parent.rotation;
        float angleToRotate = Quaternion.Angle(startRotation, desiredRotation);
        float adjustedDuration = data.LerpDuration * (angleToRotate / data.OpenAngle);
        
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);
            
            transform.parent.rotation = Quaternion.Lerp(startRotation, desiredRotation, step);
            yield return null;
        }

        transform.parent.rotation = desiredRotation;
    }
}