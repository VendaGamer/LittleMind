using UnityEngine;
using UnityEngine.InputSystem;

public class UnlockableDoor : Door
{
    [SerializeField] private DoorKey requiredKey;
    private bool isLocked = true;
    public override Interaction[] CurrentInteractions
    {
        get
        {
            if (isLocked)
            {
                return new[] { ((UnlockableDoorData)Data).UnlockDoorInteraction, Data.LookThroughKeyHoleInteraction };
            }
            return base.CurrentInteractions;
        }
    }

    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isLocked)
        {
            if (invokedAction.id == ((UnlockableDoorData)Data).UnlockDoorInteraction.ActionRef.action.id)
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