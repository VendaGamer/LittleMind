using EZCameraShake;
using UnityEngine;
public class Trembling : AnxietySymptom
{
    [SerializeField] private float baseMagnitude = 2f;
    [SerializeField] private float baseRoughness = 5f;
    [SerializeField] private float baseFadeInTime = 3f;
    [SerializeField] private float baseFadeOutTime = 5f;
    [SerializeField] private Vector3 baseRotationInfluence = new Vector3(0.7f, 0.7f, 0.7f);
    private CameraShakeInstance currentShake;
    protected override void ActivateSymptom(float intensity)
    {
        if (currentShake == null)
        {
            currentShake = CameraShaker.Instance.StartShake(baseMagnitude * Intensity, baseRoughness * Intensity, baseFadeInTime / Intensity);
            return;
        }
        currentShake.Magnitude = baseMagnitude * Intensity;
        currentShake.Roughness = baseRoughness * Intensity;
        currentShake.Magnitude = baseMagnitude * Intensity;
        currentShake.RotationInfluence = baseRotationInfluence * Intensity;
    }

    public override void StopSymptom()
    {
        enabled = false;
        currentShake.StartFadeOut(baseFadeOutTime * Intensity);
        currentShake = null;
    }
}