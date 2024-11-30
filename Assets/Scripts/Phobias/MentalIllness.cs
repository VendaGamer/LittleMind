using System.Collections;
using UnityEngine;

public abstract class MentalIllness : MonoBehaviour
{
    [SerializeField] protected float maxAnxietyLevel = 1f;
    [SerializeField] protected float anxietyBuildUpRate = 0.1f;
    [SerializeField] protected float fadeDuration = 5f; // Doba fade-out
    [SerializeField] protected Symptom[] Symptoms;

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
        if (CurrentAnxietyLevel > 0)
        {
            fadeRoutine ??= StartCoroutine(FadeOutSymptoms());
        }
    }

    /// <summary>
    /// Provede fade-out vsech aktivnich symptomu
    /// </summary>
    protected virtual IEnumerator FadeOutSymptoms()
    {
        float startAnxietyLevel = CurrentAnxietyLevel;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            CurrentAnxietyLevel = Mathf.Lerp(startAnxietyLevel, 0f, t);

            foreach (var symptom in Symptoms)
            {
                if (symptom.IsActive)
                {
                    symptom.UpdateOrTriggerSymptom(CurrentAnxietyLevel);
                }
            }

            yield return null;
        }
        
        CurrentAnxietyLevel = 0f;
    
        StopAllSymptoms();

        fadeRoutine = null;
    }
    
    protected virtual void HandleAnxiety()
    {
        if (CurrentAnxietyLevel > 0f)
        {
            UpdateSymptoms();
        }
        else
        {
            RecoverFromSymptoms();
        }
        // Reset anxiety for the next calculation cycle
        CurrentAnxietyLevel = 0f;
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
        var calculatedAnxiety = intensity * anxietyBuildUpRate + intensity;
        if (calculatedAnxiety > 0f)
        {
            var higher = Mathf.Max(CurrentAnxietyLevel, calculatedAnxiety);
            CurrentAnxietyLevel = Mathf.Min(maxAnxietyLevel, higher);
        }
    }
}
