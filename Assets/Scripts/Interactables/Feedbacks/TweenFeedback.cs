using DG.Tweening;
using DG.Tweening.Core;
using Interactables.Core;
using UnityEngine;

namespace Interactables.Feedbacks
{
    [System.Serializable]
    public class TweenFeedback : IInteractionFeedback
    {
        [SerializeField] private Transform target;

        [SerializeField] private Vector3 influence;
        
        [SerializeField] private TweenType _tweenType;
        
        [SerializeField] private int duration;

        [SerializeField] private bool isLocal;
        
        [SerializeField] private bool loop;

        public void Invoke(IInteractor interactor)
        {
            if (target == null) return;

            Tween tween = null;
            float durationInSeconds = duration / 1000f; // Convert ms to seconds if needed

            switch (_tweenType)
            {
                case TweenType.Move:
                    if (isLocal)
                        tween = target.DOLocalMove(target.localPosition + influence, durationInSeconds);
                    else
                        tween = target.DOMove(target.position + influence, durationInSeconds);
                    break;

                case TweenType.Scale:
                    tween = target.DOScale(target.localScale + influence, durationInSeconds);
                    break;

                case TweenType.Rotate:
                    if (isLocal)
                        tween = target.DOLocalRotate(target.localEulerAngles + influence, durationInSeconds);
                    else
                        tween = target.DORotate(target.eulerAngles + influence, durationInSeconds);
                    break;
            }

            if (tween != null && loop)
            {
                tween.SetLoops(-1, LoopType.Yoyo);
            }
        }
    }
    public enum TweenType
    {
        Move,
        Scale,
        Rotate
    }
}





