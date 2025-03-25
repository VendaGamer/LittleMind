using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] protected DoorInfo info;
    private Quaternion closedRotation;
    protected bool IsOpen = false;
    public string InteractGroupLabel => info.InteractableGroupLabel;
    private CancellationTokenSource canTokSrc;
    private void Start()
    {
        closedRotation = transform.parent.rotation;
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
            if (invokedAction.id == info.CloseDoorInteraction.ActionRef.action.id)
            {
                canTokSrc = canTokSrc.CancelCurrentActionAndCreateNewSrc();
                _ = RotateDoor(closedRotation, canTokSrc.Token);
                IsOpen = false;
                return true;
            }
        }
        else if(invokedAction.id == info.OpenDoorInteraction.ActionRef.action.id)
        {
            canTokSrc = canTokSrc.CancelCurrentActionAndCreateNewSrc();
            _ = RotateDoor(
                closedRotation * Quaternion.Euler(0f, info.OpenAngle, 0f),
                canTokSrc.Token);
            IsOpen = true;
            return true;
        }
        return false;
    }

    public bool ToggleOutline(bool value)
    {
        return false;
    }

    private async Task RotateDoor(Quaternion desiredRotation, CancellationToken canTok)
    {
        Quaternion startRotation = transform.parent.rotation;
        float angleToRotate = Quaternion.Angle(startRotation, desiredRotation);
        float adjustedDuration = info.LerpDuration * (angleToRotate / info.OpenAngle);
        
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            canTok.ThrowIfCancellationRequested();
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);
            
            transform.parent.rotation = Quaternion.Lerp(startRotation, desiredRotation, step);
            await Task.Yield();
        }

        transform.parent.rotation = desiredRotation;
    }
}