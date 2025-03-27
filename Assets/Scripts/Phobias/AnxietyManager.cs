using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AnxietyManager : MonoBehaviour
{
    [Header("Anxiety Settings")]
    [SerializeField] protected AnxietyManagerData Data;
    
    #region Protected props
    protected AnxietySymptom[] Symptoms;
    #endregion
    #region Private props
    private CancellationTokenSource fadeCancellationTokenSource;
    private float currentAnxiety = 0f;
    private readonly HashSet<IAnxietySource> activeAnxietySources = new();
    #endregion
    public void RegisterAnxietySource(IAnxietySource anxietySource)
    {
        fadeCancellationTokenSource?.Cancel();
        activeAnxietySources.Add(anxietySource);
        IncreaseAnxiety(anxietySource.InitialAnxietyAmount);
        OnAnxietySourcesChanged();
    }

    public void UnRegisterAnxietySource(IAnxietySource anxietySource)
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
        var previousAnxiety = CurrentAnxiety;
        CurrentAnxiety = Mathf.Clamp(currentAnxiety + InitialAnxietyAmount, 0f, Data.MaxAnxietyLevel);
    }

    /// <summary>
    /// Starts the anxiety fade out process
    /// </summary>
    protected virtual void StartFadeAnxiety()
    {
        fadeCancellationTokenSource?.Cancel();

        fadeCancellationTokenSource = new CancellationTokenSource();
        
        _ = FadeAnxietyAsync(fadeCancellationTokenSource.Token);
    }

    /// <summary>
    /// Async method to fade anxiety over time
    /// </summary>
    protected virtual async Task FadeAnxietyAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(Data.FadeOutDelayInMs, cancellationToken);
        float startAnxiety = CurrentAnxiety;
        float elapsed = 0f;
        
        while (elapsed < Data.FadeDuration)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                fadeCancellationTokenSource?.Dispose();
                return;
            }

            await Task.Yield();
            elapsed += Time.deltaTime;
            float t = elapsed / Data.FadeDuration;
            CurrentAnxiety = Mathf.Lerp(startAnxiety, 0f, t);
        }

        // Ensure anxiety reaches zero
        CurrentAnxiety = 0f;
    }
    protected virtual void OnDestroy()
    {
        fadeCancellationTokenSource?.Dispose();
    }
}

