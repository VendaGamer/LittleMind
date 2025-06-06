using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIElementDOTweenExtensions
{
    /// <summary>
    /// Tweens a VisualElement's width and height scale.
    /// </summary>
    public static Tweener DOScale(this VisualElement target, Vector2 endValue, float duration)
    {
        Vector2 startValue = new Vector2(target.resolvedStyle.width, target.resolvedStyle.height);
        return DOTween.To(
            () => startValue,
            x =>
            {
                target.style.width = x.x;
                target.style.height = x.y;
            },
            endValue,
            duration
        );
    }

    /// <summary>
    /// Tweens a VisualElement's scale using transform scale property.
    /// </summary>
    public static Tweener DOScale(this VisualElement target, float endValue, float duration)
    {
        Vector3 startValue = target.transform.scale;
        Vector3 endScale = new Vector3(endValue, endValue, 1f);

        return DOTween.To(
            () => startValue,
            x =>
            {
                target.transform.scale = x;
            },
            endScale,
            duration
        );
    }

    public static Tweener DOColor(this VisualElement target, Color endValue, float duration)
    {
        Color startValue = target.resolvedStyle.backgroundColor;
        return DOTween.To(
            () => startValue,
            x => target.style.backgroundColor = x,
            endValue,
            duration
        );
    }

    /// <summary>
    /// Tweens a VisualElement's opacity property.
    /// </summary>
    public static Tweener DOFade(this VisualElement target, float endValue, float duration)
    {
        float startValue = target.resolvedStyle.opacity;
        return DOTween.To(() => startValue, x => target.style.opacity = x, endValue, duration);
    }

    public static Tweener DOFadeOut(this VisualElement target, float duration)
    {
        float startValue = 1f;
        return DOTween
            .To(() => startValue, x => target.style.opacity = x, 0f, duration)
            .OnComplete(() =>
            {
                target.visible = false;
            });
    }

    public static Tweener DOFadeIn(this VisualElement target, float duration)
    {
        float startValue = 0f;
        return DOTween
            .To(() => startValue, x => target.style.opacity = x, 1f, duration)
            .OnStart(() =>
            {
                target.visible = true;
            });
    }

    /// <summary>
    /// Tweens a VisualElement's position.
    /// </summary>
    public static Tweener DOMove(this VisualElement target, Vector2 endValue, float duration)
    {
        Vector2 startValue = new Vector2(target.resolvedStyle.left, target.resolvedStyle.top);
        return DOTween.To(
            () => startValue,
            x =>
            {
                target.style.left = x.x;
                target.style.top = x.y;
            },
            endValue,
            duration
        );
    }
}
