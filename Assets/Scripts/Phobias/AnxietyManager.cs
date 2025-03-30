using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public abstract class AnxietyManager : MonoBehaviour
{
    [Header("Anxiety Settings")]
    [SerializeField] protected AnxietyManagerData Data;
    
    #region Protected props
    protected AnxietySymptom[] Symptoms;
    protected readonly HashSet<IAnxietySource> activeAnxietySources = new();
    [CanBeNull] protected Coroutine FadeCoroutine;
    #endregion
    #region Private props
    private float currentAnxiety = 0f;
    #endregion
    public virtual void RegisterAnxietySource(IAnxietySource anxietySource)
    {
        if (FadeCoroutine != null)
        {
            StopCoroutine(FadeCoroutine);
        }
        activeAnxietySources.Add(anxietySource);
        IncreaseAnxiety(anxietySource.InitialAnxietyAmount);
        OnAnxietySourcesChanged();
    }

    public virtual void UnRegisterAnxietySource(IAnxietySource anxietySource)
    {
        activeAnxietySources.Remove(anxietySource);
        if (activeAnxietySources.Count == 0)
        {
            StartFadeAnxiety();
        }
        OnAnxietySourcesChanged();
    }


    protected float CurrentAnxiety
    {
        get => currentAnxiety;
        set
        {
            var newValue = Mathf.Clamp(value, 0f, Data.MaxAnxietyLevel);
            if (Mathf.Approximately(newValue, currentAnxiety)) return;
            
            currentAnxiety = newValue;
            OnAnxietyValueChanged();
        }
    }
    /// <summary>
    /// Maybe will be useful, but rn idk
    /// </summary>
    protected virtual void OnAnxietySourcesChanged()
    {
        Debug.Log($"Current sources count {activeAnxietySources.Count}");
    }

    protected virtual void OnAnxietyChanged()
    {
        foreach (var symptom in Symptoms)
        {
            symptom.OnAnxietyChanged(CurrentAnxiety);
        }
    }
    protected virtual void Start()
    {
        Symptoms = GetComponentsInChildren<AnxietySymptom>();
    }

    /// <summary>
    /// Centralized method to handle anxiety value changes
    /// </summary>
    protected virtual void OnAnxietyValueChanged()
    {
        foreach (var symptom in Symptoms)
        {
            symptom.OnAnxietyChanged(CurrentAnxiety);
        }
    }
    protected virtual void Update()
    {
        if (activeAnxietySources.Count > 0)
        {
            IncreaseAnxiety(activeAnxietySources.Max(anxietySource => anxietySource.AnxietyAmount));
        }
    }

    /// <summary>
    /// Increases anxiety value if it has not reached maximum
    /// </summary>
    public virtual void IncreaseAnxiety(float InitialAnxietyAmount)
    {
        CurrentAnxiety = Mathf.Clamp(currentAnxiety + (InitialAnxietyAmount * Time.deltaTime), 0f, Data.MaxAnxietyLevel);
    }

    /// <summary>
    /// Starts the anxiety fade out process
    /// </summary>
    protected virtual void StartFadeAnxiety()
    {
        if (FadeCoroutine != null)
        {
            StopCoroutine(FadeCoroutine);
        }
        FadeCoroutine = StartCoroutine(FadeAnxietyCoroutine());
    }

    /// <summary>
    /// Async method to fade anxiety over time
    /// </summary>
    protected virtual IEnumerator FadeAnxietyCoroutine()
    {
        yield return new WaitForSeconds(Data.FadeOutDelayInMs / 1000f);
        float startAnxiety = CurrentAnxiety;
        float elapsed = 0f;
        
        while (elapsed < Data.FadeDuration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            float t = elapsed / Data.FadeDuration;
            CurrentAnxiety = Mathf.Lerp(startAnxiety, 0f, t);
        }

        // Ensure anxiety reaches zero
        CurrentAnxiety = 0f;
        StopAllSymtoms();
        FadeCoroutine = null;
    }

    protected void StopAllSymtoms()
    {
        foreach (var symptom in Symptoms)
        {
            symptom.StopSymptom();
        }
    }
    protected void StartAllSymtoms()
    {
        foreach (var symptom in Symptoms)
        {
            symptom.ActivateSymptom(CurrentAnxiety);
        }
    }
}

