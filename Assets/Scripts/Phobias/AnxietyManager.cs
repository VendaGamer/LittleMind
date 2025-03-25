using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AnxietyManager : MonoBehaviour
{
    [Header("Anxiety Settings")] 
    [SerializeField] protected float MaxAnxietyLevel = 1f;
    [SerializeField] protected float FadeDuration = 20f;
    [SerializeField] protected float FadeOutDelay = 5f;

    protected AnxietySymptom[] Symptoms;

    private float currentAnxiety = 0f;
    private float lastAnxiety;
    private float fadeOutTime;
    private CancellationTokenSource fadeCancellationTokenSource;
    private bool wasAnxious = false;

    public float CurrentAnxiety
    {
        get => currentAnxiety;
        protected set
        {
            var newValue = Mathf.Clamp(value, 0f, MaxAnxietyLevel);
            if (!Mathf.Approximately(newValue, currentAnxiety))
            {
                currentAnxiety = newValue;
                OnAnxietyValueChanged(currentAnxiety);
            }
        }
    }
    
    public event Action<float> AnxietyChanged;

    protected virtual void Start()
    {
        Symptoms = GetComponentsInChildren<AnxietySymptom>();
        foreach (var symptom in Symptoms)
        {
            AnxietyChanged += symptom.OnAnxietyChanged;
        }
        
        // Initialize fade out time
        fadeOutTime = FadeOutDelay;
    }

    /// <summary>
    /// Centralized method to handle anxiety value changes
    /// </summary>
    protected virtual void OnAnxietyValueChanged(float newAnxiety)
    {
        // Trigger event
        AnxietyChanged?.Invoke(newAnxiety);

        // Check if anxiety state has changed
        bool isCurrentlyAnxious = newAnxiety > 0;
        if (isCurrentlyAnxious != wasAnxious)
        {
            if (isCurrentlyAnxious)
            {
                // Anxiety is starting
                foreach (var symptom in Symptoms)
                {
                    symptom.enabled = true;
                }
            }
            else
            {
                // Anxiety is ending
                foreach (var symptom in Symptoms)
                {
                    symptom.StopSymptom();
                }
            }

            // Update the anxiety state
            wasAnxious = isCurrentlyAnxious;
        }
    }

    protected virtual void LateUpdate()
    {
        // Check if anxiety is not increasing
        bool isAnxietyStable = Mathf.Approximately(lastAnxiety, CurrentAnxiety);
        
        // Determine if we should start fade out
        if (isAnxietyStable && CurrentAnxiety > 0)
        {
            fadeOutTime -= Time.deltaTime;

            // Start fade out if delay has passed
            if (fadeOutTime <= 0)
            {
                StartFadeAnxiety();
            }
        }
        else
        {
            // Reset fade out time when anxiety changes or increases
            fadeOutTime = FadeOutDelay;
        }
    }

    protected virtual void Update()
    {
        lastAnxiety = CurrentAnxiety;
    }

    /// <summary>
    /// Increase anxiety to a new value (if higher) and restart the fade timer.
    /// </summary>
    public virtual void IncreaseAnxiety(float amount)
    {
        float previousAnxiety = CurrentAnxiety;
        CurrentAnxiety = Mathf.Clamp(currentAnxiety + amount, 0f, MaxAnxietyLevel);
        
        // If anxiety has increased, cancel ongoing fade and reset timer
        if (CurrentAnxiety > previousAnxiety)
        {
            fadeCancellationTokenSource?.Cancel();
            fadeOutTime = FadeOutDelay;
        }
    }

    /// <summary>
    /// Start the async anxiety fade out process
    /// </summary>
    protected virtual void StartFadeAnxiety()
    {
        // Cancel any existing fade task
        fadeCancellationTokenSource?.Cancel();
        
        // Create a new cancellation token source
        fadeCancellationTokenSource = new CancellationTokenSource();
        
        // Start the fade task
        _ = FadeAnxietyAsync(fadeCancellationTokenSource.Token);
    }

    /// <summary>
    /// Async method to fade anxiety over time
    /// </summary>
    protected virtual async Task FadeAnxietyAsync(CancellationToken cancellationToken)
    {
        float startAnxiety = CurrentAnxiety;
        float elapsed = 0f;

        try 
        {
            while (elapsed < FadeDuration)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
                
                elapsed += Time.deltaTime;
                float t = elapsed / FadeDuration;
                CurrentAnxiety = Mathf.Lerp(startAnxiety, 0f, t);
            }

            // Ensure anxiety reaches zero
            CurrentAnxiety = 0f;
        }
        catch (OperationCanceledException)
        {
            // Fade was interrupted, do nothing special
        }
        finally
        {
            // Clean up the cancellation token source
            fadeCancellationTokenSource?.Dispose();
            fadeCancellationTokenSource = null;
        }
    }

    /// <summary>
    /// Directly set anxiety value
    /// </summary>
    protected virtual void SetAnxiety(float value)
    {
        CurrentAnxiety = Mathf.Clamp(value, 0f, MaxAnxietyLevel);
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe from events
        foreach (var symptom in Symptoms)
        {
            AnxietyChanged -= symptom.OnAnxietyChanged;
        }

        // Cancel any ongoing fade
        fadeCancellationTokenSource?.Cancel();
    }
}