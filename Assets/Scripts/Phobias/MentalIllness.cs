using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MentalIllness : MonoBehaviour
{
    [SerializeField] protected float maxAnxietyLevel = 1f;
    [SerializeField] protected float anxietyBuildUpAndRecoveryRate = 0.1f;
    [SerializeField] protected float fadeDuration = 5f; // Doba fade-out
    protected readonly List<Symptom> Symptoms = new();

    protected float CurrentAnxietyLevel = 0f;
    protected Coroutine fadeRoutine;

    public float AnxietyLevel => CurrentAnxietyLevel;

    /// <summary>
    /// Logika pro aktivitu danych symptomu, ve velke vetsine neovverridnu
    /// </summary>
    protected virtual void UpdateSymptoms()
    {
        foreach (var symptom in Symptoms)
        {
            symptom.UpdateOrTriggerSymptom(CurrentAnxietyLevel);
        }
    }

    /// <summary>
    /// Logika na postupne zastaveni (fadenuti) symptomu
    /// </summary>
    protected virtual void RecoverFromSymptoms()
    {
        fadeRoutine ??= StartCoroutine(FadeOutSymptoms());
    }

    /// <summary>
    /// Provede fade-out vsech aktivnich symptomu
    /// </summary>
    protected virtual IEnumerator FadeOutSymptoms()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float fadeProgress = 1f - (elapsed / fadeDuration);

            foreach (var symptom in Symptoms)
            {
                if (symptom.IsActive)
                {
                    symptom.UpdateOrTriggerSymptom(CurrentAnxietyLevel);
                }
            }

            yield return null;
        }
        
        StopAllSymptoms();

        fadeRoutine = null;
    }

    protected void StopAllSymptoms()
    {
        foreach (var symptom in Symptoms)
        {
            if (symptom.IsActive)
            {
                symptom.StopSymptom();
            }
        }
    }

    /// <summary>
    /// Nastavi intesitu uzkosti, pokud jiz neni vetsi nebo nepresahuje maximalni
    /// </summary>
    /// <param name="intensity"></param>
    public void PendNewAnxietyLevel(float intensity)
    {
        var calculatedAnxiety = intensity * anxietyBuildUpAndRecoveryRate;
        if (calculatedAnxiety > 0f)
        {
            var higher = Mathf.Max(CurrentAnxietyLevel, calculatedAnxiety);
            CurrentAnxietyLevel = Mathf.Min(maxAnxietyLevel, higher);
        }
    }

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
        Symptoms.Add(GetComponent<T>()); //pridame do listu symptomu
    }
}
