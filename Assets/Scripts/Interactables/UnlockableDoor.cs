using UnityEngine;
using UnityEngine.InputSystem;

public class UnlockableDoor : Door
{
    [SerializeField] private DoorKey requiredKey;

    protected bool _isLocked = true;
    
    protected bool IsLocked
    {
        get => _isLocked;
        set
        {
            if (value == _isLocked)
                return;
            _isLocked = value;
            OnInteractionsChanged();
        }
    }
    
    public override Interaction[] CurrentInteractions
    {
        get
        {
            if (IsLocked)
            {
                return new[] { ((UnlockableDoorData)data).UnlockDoorInteraction, data.LookThroughKeyHoleInteraction };
            }
            return base.CurrentInteractions;
        }
    }

    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (IsLocked)
        {
            if (invokedAction.id == ((UnlockableDoorData)data).UnlockDoorInteraction.Action.id)
            {
                if (ReferenceEquals(interactor.InteractableHolding, requiredKey))
                {
                    IsLocked = false;
                    return true;
                }
                return false;
            }
            return false;
        }
        
        return base.Interact(interactor, invokedAction);
    }
}