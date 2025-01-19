using UnityEngine;
using UnityEngine.InputSystem;

public class UnlockableDoor : Door
{
    [SerializeField] private DoorKey requiredKey;
    [SerializeField] private Interaction unlockDoorInteraction;
    private bool isLocked = true;
    public override Interaction[] CurrentInteractions
    {
        get
        {
            if (isLocked)
            {
                return new[] { unlockDoorInteraction, lookThroughKeyHoleInteraction };
            }
            return base.CurrentInteractions;
        }
    }

    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isLocked)
        {
            if (invokedAction.id == unlockDoorInteraction.Action.action.id)
            {
                if (interactor.InteractableHolding == requiredKey)
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