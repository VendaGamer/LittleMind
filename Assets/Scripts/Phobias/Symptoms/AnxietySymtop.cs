using UnityEngine;

public abstract class AnxietySymptom : MonoBehaviour
{
    protected float Intensity { get; set; } = 0f;

    /// <summary>
    /// Called when the anxiety level changes.
    /// </summary>
    public virtual void OnAnxietyChanged(float newAnxiety)
    {
        Intensity = newAnxiety;
    }

    /// <summary>
    /// Implement this method to define how the symptom activates.
    /// </summary>
    public abstract void ActivateSymptom(float intensity);

    /// <summary>
    /// Optionally override to define cleanup behavior.
    /// </summary>
    protected virtual void DeactivateSymptom()
    {
        enabled = false;
        Intensity = 0f;
        StopSymptom();
    }

    /// <summary>
    /// Stop the symptom (e.g. end any coroutines or animations).
    /// </summary>
    public abstract void StopSymptom();
}
