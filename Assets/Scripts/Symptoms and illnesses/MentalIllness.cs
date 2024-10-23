using System.Collections.Generic;
using UnityEngine;

public abstract class MentalIllness : MonoBehaviour
{
    [SerializeField] protected List<Symptom> symptoms = new();
    [SerializeField] protected float triggerDistance = 5f;
    [SerializeField] protected float maxAnxietyLevel = 1f;
    [SerializeField] protected float anxietyBuildupRate = 1f;

    protected float currentAnxietyLevel = 0f;

    public float AnxietyLevel => currentAnxietyLevel;
    /// <summary>
    /// Tuto metodu budu <see langword="override"/>, pro to abych spoustel aktivoval jednotlivej symptom
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (currentAnxietyLevel > 0f)
        {
            UpdateSymptoms();
        }
        else
        {
            RecoverFromSymptoms();
        }
        currentAnxietyLevel = 0f;
    }
    /// <summary>
    /// Logika pro aktivitu danych symptomu, ve velke vetsine neovverridnu
    /// </summary>
    protected virtual void UpdateSymptoms()
    {
        foreach (var symptom in symptoms)
        {
            symptom.UpdateOrTriggerSymptom(currentAnxietyLevel);
        }
    }
    /// <summary>
    /// logika na postupne zastaveni (fadenuti) ze symptomu
    /// </summary>
    protected virtual void RecoverFromSymptoms()
    {
        foreach (var symptom in symptoms)
        {
            if (symptom.IsActive)
            {
                symptom.StopSymptom();
            }
        }
    }

    /// <summary>
    /// Logika pro to jak se dana fobie budi a jak budi sve symptomy
    /// </summary>
    /// <param name="intensity"></param>
    public abstract void PendNewAnxietyLevel(float intensity);
    /// <summary>
    /// Prida symptom hracovi, pokud ho uz nema
    /// </summary>
    /// <typeparam name="T"></typeparam>
    protected void RequireSymptom<T>() where T : Symptom
    {
        if (!GetComponent<T>())
        {
            gameObject.AddComponent<T>();
        }
        symptoms.Add(GetComponent<T>()); //pridame do listu symptomu
    }
}