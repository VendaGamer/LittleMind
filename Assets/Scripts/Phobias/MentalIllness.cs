using System.Collections.Generic;
using Symptoms;
using UnityEngine;

public abstract class MentalIllness : MonoBehaviour
{
    [SerializeField] protected float maxAnxietyLevel = 1f;
    [SerializeField] protected float anxietyBuildupRate = 1f;
    protected List<Symptom> Symptoms = new();

    protected float CurrentAnxietyLevel = 0f;

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
    /// logika na postupne zastaveni (fadenuti) ze symptomu
    /// </summary>
    protected virtual void RecoverFromSymptoms()
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
    /// Nastavi intesitu uskosti, pokud jiz neni vetsi nebo nepresahuje maximalni
    /// </summary>
    /// <param name="intensity"></param>
    public void PendNewAnxietyLevel(float intensity)
    {
        var calculatedAnxiety = intensity * anxietyBuildupRate;
        if (calculatedAnxiety > 0f)
        {
            var higher = Mathf.Max(CurrentAnxietyLevel, calculatedAnxiety);
            CurrentAnxietyLevel = Mathf.Min(maxAnxietyLevel, higher);
            Debug.Log("Set new anxiety: " + CurrentAnxietyLevel);
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