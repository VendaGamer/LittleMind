using UnityEngine;

public abstract class AnxietySymptom : MonoBehaviour
{
    [Header("Symptom Settings")]
    [SerializeField] protected float MinimalIntensity = 0.1f;

    protected bool IsActive { get; set; } = false;
    protected float Intensity { get; set; } = 0f;

    /// <summary>
    /// Called when the anxiety level changes.
    /// </summary>
    public virtual void OnAnxietyChanged(float newAnxiety)
    {
        if (newAnxiety >= MinimalIntensity)
        {
            IsActive = true;
            Intensity = newAnxiety;
            ActivateSymptom(newAnxiety);
        }
        else
        {
            DeactivateSymptom();
        }
    }

    /// <summary>
    /// Implement this method to define how the symptom activates.
    /// </summary>
    protected abstract void ActivateSymptom(float intensity);

    /// <summary>
    /// Optionally override to define cleanup behavior.
    /// </summary>
    protected virtual void DeactivateSymptom()
    {
        IsActive = false;
        Intensity = 0f;
        StopSymptom();
    }

    /// <summary>
    /// Stop the symptom (e.g. end any coroutines or animations).
    /// </summary>
    public virtual void StopSymptom() { }
}