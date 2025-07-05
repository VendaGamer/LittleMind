using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class RotateFeedback : IInteractionFeedback
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;
    [SerializeField] private bool isLocalRotation = true;

    public void Execute(Interactable interactable, IInteractor interactor)
    {
        if (isLocalRotation)
        {
            target.DOLocalRotate(targetRotation, duration).SetEase(easeType);
        }
        else
        {
            target.DORotate(targetRotation, duration).SetEase(easeType);
        }
    }
}