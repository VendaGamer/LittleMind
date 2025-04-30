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

    /// <summary>
    /// Creates a heartbeat effect for a VisualElement.
    /// </summary>
    public static Sequence DOHeartbeat(
        this VisualElement target,
        float intensity = 1.15f,
        float duration = 0.5f
    )
    {
        Sequence sequence = DOTween.Sequence();

        // First beat - quick expansion
        sequence.Append(target.DOScale(intensity, duration * 0.2f).SetEase(Ease.OutQuad));

        // Quick contraction
        sequence.Append(target.DOScale(0.95f, duration * 0.15f).SetEase(Ease.InOutQuad));

        // Second beat - smaller expansion
        sequence.Append(target.DOScale(intensity * 0.8f, duration * 0.15f).SetEase(Ease.OutQuad));

        // Return to normal
        sequence.Append(target.DOScale(1f, duration * 0.5f).SetEase(Ease.InOutQuad));

        return sequence;
    }

    /// <summary>
    /// Creates a continuous heartbeat effect that can be stopped later.
    /// </summary>
    public static Sequence DOHeartbeatContinuous(
        this VisualElement target,
        float bpm = 60f,
        float intensity = 1.15f
    )
    {
        float beatDuration = 60f / bpm;

        Sequence sequence = DOTween.Sequence();

        // First beat - quick expansion
        sequence.Append(target.DOScale(intensity, beatDuration * 0.1f).SetEase(Ease.OutQuad));

        // Quick contraction
        sequence.Append(target.DOScale(0.95f, beatDuration * 0.1f).SetEase(Ease.InOutQuad));

        // Second beat - smaller expansion
        sequence.Append(
            target.DOScale(intensity * 0.8f, beatDuration * 0.1f).SetEase(Ease.OutQuad)
        );

        // Return to normal with a longer pause
        sequence.Append(target.DOScale(1f, beatDuration * 0.2f).SetEase(Ease.InOutQuad));

        // Add pause between beats
        sequence.AppendInterval(beatDuration * 0.5f);

        // Loop the sequence
        sequence.SetLoops(-1); // -1 means infinite loops

        return sequence;
    }

    /// <summary>
    /// Creates a more realistic heartbeat effect with dynamic intensity based on heart rate.
    /// </summary>
    public static Sequence DORealisticHeartbeat(
        this VisualElement target,
        float bpm,
        float minIntensity = 1.05f,
        float maxIntensity = 1.3f
    )
    {
        // Calculate intensity based on heart rate
        float normalizedHeartRate = Mathf.InverseLerp(60f, 180f, bpm); // Map 60-180 bpm to 0-1
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedHeartRate);

        // Calculate timing based on BPM
        float beatDuration = 60f / bpm;
        float lubDuration = beatDuration * 0.15f;
        float dubDuration = beatDuration * 0.15f;
        float lubDubGap = beatDuration * 0.05f;
        float restDuration = beatDuration - (lubDuration + dubDuration + lubDubGap);

        Sequence sequence = DOTween.Sequence();

        // Lub: First stronger beat
        sequence.Append(target.DOScale(intensity, lubDuration * 0.5f).SetEase(Ease.OutQuad));
        sequence.Append(target.DOScale(0.98f, lubDuration * 0.5f).SetEase(Ease.InOutQuad));

        // Small pause between lub and dub
        sequence.AppendInterval(lubDubGap);

        // Dub: Second weaker beat
        sequence.Append(target.DOScale(intensity * 0.8f, dubDuration * 0.5f).SetEase(Ease.OutQuad));
        sequence.Append(target.DOScale(1f, dubDuration * 0.5f).SetEase(Ease.InOutQuad));

        // Rest until next heartbeat
        sequence.AppendInterval(restDuration);

        // Loop the sequence
        sequence.SetLoops(-1);

        return sequence;
    }

    /// <summary>
    /// Tweens a VisualElement's color property.
    /// </summary>
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
