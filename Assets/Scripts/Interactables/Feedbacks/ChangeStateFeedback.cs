// ChangeStateResponse.cs
using UnityEngine;

[System.Serializable]

public class ChangeStateResponse :  IInteractionFeedback
{
    [Tooltip("The new state to switch to.")]
    [SerializeReference] 
    private int targetStateIndex;

    public void Execute(Interactable interactable, IInteractor interactor)
    {
        if (interactable.InteractionProvider is StatefulInteractionProvider statefulInteractionProvider)
        {
            statefulInteractionProvider.ChangeState(targetStateIndex);
        }
    }
}