using System;
using System.Collections;
using UnityEngine;

public abstract class AnxietyManager : MonoBehaviour
{
    [Header("Anxiety Settings")] [SerializeField]
    protected float MaxAnxietyLevel = 1f;

    [SerializeField] protected float FadeDuration = 5f;
    protected AnxietySymptom[] Symptoms;

    private float currentAxiety = 0f;

    public float CurrentAnxiety
    {
        get => currentAxiety;
        protected set
        {
            var newValue = Mathf.Clamp(value, 0f, MaxAnxietyLevel);
            if (!Mathf.Approximately(newValue, currentAxiety))
            {
                currentAxiety = newValue;
                AnxietyChanged?.Invoke(value);
            }
        }
    }
    
    public event Action<float> AnxietyChanged;

    private Coroutine fadeRoutine;

    protected virtual void Start()
    {
        Symptoms = GetComponentsInChildren<AnxietySymptom>();
        foreach (var symptom in Symptoms)
        {
            AnxietyChanged += symptom.OnAnxietyChanged;
        }
    }

    /// <summary>
    /// Increase anxiety to a new value (if higher) and restart the fade timer.
    /// </summary>
    public virtual void IncreaseAnxiety(float amount)
    {
        if (amount <= 0f)
            return;

        // Only increase if the new amount is higher
        CurrentAnxiety = Mathf.Clamp(Mathf.Max(CurrentAnxiety, amount), 0f, MaxAnxietyLevel);

        // Restart fade routine if one is already running
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        fadeRoutine = StartCoroutine(FadeAnxiety());
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual IEnumerator FadeAnxiety()
    {
        float startAnxiety = CurrentAnxiety;
        float elapsed = 0f;

        while (elapsed < FadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / FadeDuration;
            CurrentAnxiety = Mathf.Lerp(startAnxiety, 0f, t);
            yield return null;
        }

        CurrentAnxiety = 0f;
        fadeRoutine = null;
    }

    /// <summary>
    /// Nastavovat budeme pouze v tehle 
    /// </summary>
    protected virtual void SetAnxiety(float value)
    {
        CurrentAnxiety = Mathf.Clamp(value, 0f, MaxAnxietyLevel);
    }

    protected virtual void OnDestroy()
    {
        foreach (var symptom in Symptoms)
        {
            AnxietyChanged -= symptom.OnAnxietyChanged;
        }
    }
}