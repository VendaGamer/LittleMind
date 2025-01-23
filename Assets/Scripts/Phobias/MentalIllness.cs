using System.Collections;
using UnityEngine;

public abstract class MentalIllness : MonoBehaviour
{
    [SerializeField] protected float maxAnxietyLevel = 1f;
    [SerializeField] protected float anxietyBuildUpRate = 0.1f;
    [SerializeField] protected float fadeDuration = 5f; // Doba fade-out
    protected Symptom[] Symptoms;

    protected float CurrentAnxietyLevel = 0f;
    private Coroutine fadeRoutine;
    protected virtual void Start()
    {
        Symptoms = GetComponents<Symptom>();
    }
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
    protected void RecoverFromSymptoms()
    {
        fadeRoutine ??= StartCoroutine(FadeOutSymptoms());
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
            elapsed += Time.fixedDeltaTime;
            Debug.Log("Elapsed time: "+elapsed);
            float t = elapsed / fadeDuration;
            CurrentAnxietyLevel = Mathf.Lerp(startAnxietyLevel, 0f, t);

            foreach (var symptom in Symptoms)
            {
                if (symptom.IsActive)
                {
                    symptom.UpdateOrTriggerSymptom(CurrentAnxietyLevel);
                }
            }

            yield return new WaitForFixedUpdate();
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
        if (intensity > 0f)
        {
            var higher = Mathf.Max(CurrentAnxietyLevel, intensity);
            CurrentAnxietyLevel = Mathf.Min(maxAnxietyLevel, higher);
        }
    }
}