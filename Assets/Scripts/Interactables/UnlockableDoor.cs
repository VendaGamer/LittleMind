using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnlockableDoor : Door
{
    [SerializeField] private DoorKey requiredKey;
    private bool isLocked = true;
    [CreateProperty]
    public override Interaction[] CurrentInteractions
    {
        get
        {
            if (isLocked)
            {
                return new[] { ((UnlockableDoorInfo)info).UnlockDoorInteraction, info.LookThroughKeyHoleInteraction };
            }
            return base.CurrentInteractions;
        }
    }

    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isLocked)
        {
            if (invokedAction.id == ((UnlockableDoorInfo)info).UnlockDoorInteraction.ActionRef.action.id)
            {
                if (ReferenceEquals(interactor.InteractableHolding, requiredKey))
                {
                    isLocked = false;
                    return true;
                }
                return false;
            }
            return false;
        }
        
        return base.Interact(interactor, invokedAction);
    }
}