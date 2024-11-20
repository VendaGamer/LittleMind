using UnityEngine;

public class VisualDistortion : Symptom
{
    public override bool HasSelfManagedFadeOutAndFadeIn { get; protected set; } = false;
    [SerializeField] private float maxDistortionAmount = 0.5f;

    public override void StopSymptom()
    {
        IsActive = false;
        Intensity = 0f;
    }

    public override void UpdateOrTriggerSymptom(float intensity)
    {
        Intensity = Mathf.Lerp(Intensity, maxDistortionAmount * Intensity, Time.deltaTime);
    }
}
