using UnityEngine;
public abstract class Symptom : MonoBehaviour
{
    [SerializeField] public float Intensity { get; protected set; } = 1f;
    [SerializeField] protected float minimalIntensity = 0.5f;
    public bool IsActive { get; protected set; } = false;
    /// <summary>
    /// Provadi logiku symptomu napr. (Trembling - zacne trast kamerou a i hracem, coz se projevi na pohybu)
    /// </summary>
    public abstract void UpdateOrTriggerSymptom(float intensity);
    /// <summary> 
    /// Vypne provadeni logiky symptomu
    /// </summary>
    public virtual void StopSymptom()
    {
        IsActive = false;
        Intensity = 0f;
    }
}
