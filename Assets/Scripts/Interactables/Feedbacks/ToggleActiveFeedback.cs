// ToggleActiveResponse.cs
using UnityEngine;

[System.Serializable]
public class ToggleActiveFeedback : IInteractionFeedback
{
    [SerializeField] private GameObject target;

    public void Execute(Interactable interactable, IInteractor interactor)
    {
        if (target)
        {
            target.SetActive(!target.activeSelf);
        }
    }
}