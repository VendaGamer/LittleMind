using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactGroupLabel;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private Interaction openDoorInteraction;
    [SerializeField] private Interaction closeDoorInteraction;
    [SerializeField] protected Interaction lookThroughKeyHoleInteraction;
    
    private Quaternion closedRotation;
    private Quaternion openRotation;


    private void Start()
    {
        closedRotation = transform.parent.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    protected bool isOpen = false;
    public string InteractGroupLabel => interactGroupLabel;

    public virtual Interaction[] CurrentInteractions
    {
        get
        {
            return isOpen ? new[] { closeDoorInteraction } : new[] { openDoorInteraction, lookThroughKeyHoleInteraction };
        }
    }

    public virtual bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isOpen)
        {
            if (invokedAction.id == closeDoorInteraction.Action.action.id)
            {
                CloseDoor();
                isOpen = false;
                return true;
            }
        }
        else
        {
            OpenDoor();
            isOpen = true;
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
        float adjustedDuration = rotationDuration * (angleToRotate / openAngle);
    
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
        float adjustedDuration = rotationDuration * (angleToRotate / openAngle);
    
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