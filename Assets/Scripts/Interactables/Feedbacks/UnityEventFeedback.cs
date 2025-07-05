using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventFeedback : IInteractionFeedback
{
    [SerializeField] private UnityEvent<Interactable, IInteractor> onExecute;

    public void Execute(Interactable interactable, IInteractor interactor)
    {
        onExecute?.Invoke(interactable, interactor);
    }
}