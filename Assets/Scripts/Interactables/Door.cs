using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Serialization;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] protected DoorInfo info;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    protected bool IsOpen = false;
    public string InteractGroupLabel => info.InteractableGroupLabel;
    private void Start()
    {
        closedRotation = transform.parent.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, info.OpenAngle, 0f);
    }

    public virtual Interaction[] CurrentInteractions
    {
        get
        {
            return IsOpen ? new[] { info.CloseDoorInteraction } : new[] { info.OpenDoorInteraction, info.LookThroughKeyHoleInteraction };
        }
    }

    public virtual bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsOpen)
        {
            if (invokedAction.id == info.CloseDoorInteraction.Action.action.id)
            {
                CloseDoor();
                IsOpen = false;
                return true;
            }
        }
        else if(invokedAction.id == info.OpenDoorInteraction.Action.action.id)
        {
            OpenDoor();
            IsOpen = true;
            return true;
        }
        return false;
    }

    private void OpenDoor()
    {
        StopAllCoroutines();
        StartCoroutine(OpenDoorRoutine());
    }

    private void CloseDoor()
    {
        StopAllCoroutines();
        StartCoroutine(CloseDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        Quaternion startRotation = transform.parent.rotation;
        float angleToRotate = Quaternion.Angle(startRotation, openRotation);
        float adjustedDuration = info.LerpDuration * (angleToRotate / info.OpenAngle);
    
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);

            transform.parent.rotation = Quaternion.Lerp(startRotation, openRotation, step);
            yield return null;
        }

        transform.parent.rotation = openRotation;
    }

    private IEnumerator CloseDoorRoutine()
    {
        Quaternion startRotation = transform.parent.rotation;
        float angleToRotate = Quaternion.Angle(startRotation, closedRotation);
        float adjustedDuration = info.LerpDuration * (angleToRotate / info.OpenAngle);
    
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);

            transform.parent.rotation = Quaternion.Lerp(startRotation, closedRotation, step);
            yield return null;
        }

        transform.parent.rotation = closedRotation;
    }
}