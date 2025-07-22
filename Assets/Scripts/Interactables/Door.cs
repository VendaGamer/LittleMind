using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Door : BaseInteractable
{
    [FormerlySerializedAs("info")] [SerializeField] protected DoorData data;
    
    protected override InteractableData InteractableData => data;
    private Quaternion closedRotation;
    private bool _isOpen = false;

    protected bool IsOpen
    {
        get => _isOpen;
        set
        {
            if (value == _isOpen)
                return;
            _isOpen = value;
            OnInteractionsChanged();
        }
    }
    
    private Coroutine currentRotateCoroutine;
    
    protected override void Start()
    {
        closedRotation = transform.parent.rotation;
    }
    
    public override Interaction[] CurrentInteractions
    {
        get
        {
            return IsOpen ? new[] { data.CloseDoorInteraction } : new[] { data.OpenDoorInteraction, data.LookThroughKeyHoleInteraction };
        }
    }

    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsOpen)
        {
            if (invokedAction.id == data.CloseDoorInteraction.Action.id)
            {
                if (currentRotateCoroutine != null)
                    StopCoroutine(currentRotateCoroutine);
                    
                currentRotateCoroutine = StartCoroutine(RotateDoor(closedRotation));
                IsOpen = false;
                return true;
            }
        }
        else if(invokedAction.id == data.OpenDoorInteraction.Action.id)
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

    private IEnumerator RotateDoor(Quaternion desiredRotation)
    {
        var startRotation = transform.parent.rotation;
        var angleToRotate = Quaternion.Angle(startRotation, desiredRotation);
        var adjustedDuration = data.LerpDuration * (angleToRotate / data.OpenAngle);
        
        var elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            var step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);
            
            transform.parent.rotation = Quaternion.Lerp(startRotation, desiredRotation, step);
            yield return null;
        }

        transform.parent.rotation = desiredRotation;
    }
}