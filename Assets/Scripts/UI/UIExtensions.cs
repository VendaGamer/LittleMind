using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIExtensions
{
    public static VisualElement DOScale(Vector3 desiredScale, float speed)
    {
        var t = DOTween.To(
            (DOGetter<Vector3>)(() => target.position),
            (DOSetter<Vector3>)(x => target.position = x),
            endValue,
            duration
        );
        t.SetOptions(snapping).SetTarget<Tweener>((object)target);
        return t;
    }
}
