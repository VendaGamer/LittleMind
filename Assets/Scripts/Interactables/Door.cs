using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] protected DoorData Data;
    private Quaternion closedRotation;
    protected bool IsOpen = false;
    public string InteractGroupLabel => Data.InteractableGroupLabel;
    private Coroutine currentRotateCoroutine;
    
    private void Start()
    {
        closedRotation = transform.parent.rotation;
    }

    public virtual Interaction[] CurrentInteractions
    {
        get
        {
            return IsOpen ? new[] { Data.CloseDoorInteraction } : new[] { Data.OpenDoorInteraction, Data.LookThroughKeyHoleInteraction };
        }
    }

    public virtual bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsOpen)
        {
            if (invokedAction.id == Data.CloseDoorInteraction.ActionRef.action.id)
            {
                if (currentRotateCoroutine != null)
                    StopCoroutine(currentRotateCoroutine);
                    
                currentRotateCoroutine = StartCoroutine(RotateDoor(closedRotation));
                IsOpen = false;
                return true;
            }
        }
        else if(invokedAction.id == Data.OpenDoorInteraction.ActionRef.action.id)
        {
            if (currentRotateCoroutine != null)
                StopCoroutine(currentRotateCoroutine);
                
            currentRotateCoroutine = StartCoroutine(RotateDoor(
                closedRotation * Quaternion.Euler(0f, Data.OpenAngle, 0f)));
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
        float adjustedDuration = Data.LerpDuration * (angleToRotate / Data.OpenAngle);
        
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