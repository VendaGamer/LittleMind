using UnityEngine;
using EZCameraShake;
public class Trembling : Symptom
{
    [SerializeField] private float baseMagnitude = 0.8f;
    [SerializeField] private float baseRoughness = 13f;
    [SerializeField] private float baseFadeInTime = 3f;
    [SerializeField] private float baseFadeOutTime = 5f;
    [SerializeField] private Vector3 baseRotationInfluence = new Vector3(0.7f, 0.7f, 0.7f);
    [SerializeField] private Vector3 basePositionInfluence = Vector3.zero;
    private CameraShakeInstance currentShake;
    public override void StopSymptom()
    {
        IsActive = false;
        currentShake.StartFadeOut(baseFadeOutTime * Intensity);
        currentShake = null;
    }
    public override void UpdateOrTriggerSymptom(float intensity)
    {
        if (intensity < MinimalIntensity) return;
        Intensity = intensity;
        IsActive = true;
        if (currentShake == null)
        {
            Debug.Log("New shake");
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