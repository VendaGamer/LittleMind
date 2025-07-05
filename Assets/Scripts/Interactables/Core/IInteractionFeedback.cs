using UnityEngine;

public interface IInteractionFeedback
{
    void Execute(Interactable interactable, IInteractor interactor);
}