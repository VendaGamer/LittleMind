using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Dizzyness : AnxietySymptom
{
    private Camera playerCamera => PlayerCamera.Instance.Camera;
    private float originalFov;
    private TweenerCore<float, float, FloatOptions> currentCycle;

    public override void ActivateSymptom(float intensity)
    {
        originalFov = playerCamera.fieldOfView;
        Intensity = intensity;
        currentCycle = playerCamera
            .DOFieldOfView(originalFov + (30 * Intensity), 2f)
            .OnComplete(() =>
            {
                playerCamera.DOFieldOfView(originalFov, 2f);
            })
            .SetLoops(-1);
    }

    public override void StopSymptom()
    {
        currentCycle.Kill();
        playerCamera.DOFieldOfView(originalFov, 2f);
    }
}
