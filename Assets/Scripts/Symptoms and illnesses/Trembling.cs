using UnityEngine;
using EZCameraShake;
public class Trembling : Symptom
{
    [SerializeField] private float baseMagnitude = 1f;
    [SerializeField] private float baseRoughness = 17f;
    [SerializeField] private float baseFadeInTime = 1f;
    [SerializeField] private float baseFadeOutTime = 0.5f;
    [SerializeField] private Vector3 baseRotationInfluence = new Vector3(0.7f, 0.7f, 0.7f);
    [SerializeField] private Vector3 basePositionInfluence = Vector3.zero;
    private CameraShakeInstance currentShake;
    public override void StopSymptom()
    {
        IsActive = false;
        currentShake.StartFadeOut(baseFadeOutTime * Intensity);
    }
    public override void UpdateOrTriggerSymptom(float intensity)
    {
        if (intensity < MinimalIntensity) return;
        Intensity = intensity;
        IsActive = true;
        if (currentShake == null)
        {
            currentShake = CameraShaker.Instance.StartShake(baseMagnitude * Intensity, baseRoughness * Intensity, baseFadeInTime / Intensity,
            basePositionInfluence * Intensity, baseRotationInfluence * Intensity);
            return;
        }
        currentShake.Magnitude = baseMagnitude * Intensity;
        currentShake.Roughness = baseRoughness * Intensity;
        currentShake.Magnitude = baseMagnitude * Intensity;
        currentShake.PositionInfluence = basePositionInfluence * Intensity;
        currentShake.PositionInfluence = baseRotationInfluence * Intensity;
    }
}