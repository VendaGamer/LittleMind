using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class HeartBeatSymptom : AnxietySymptom
{
    [Header("UI")]
    [SerializeField]
    private UIDocument playerUI;

    private VisualElement heartElement;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip HeartBeatLub;

    [SerializeField]
    private AudioClip HeartBeatDub;

    [Header("Timing Settings")]
    [SerializeField]
    private float MinHeartRate = 60f; // beats per minute

    [SerializeField]
    private float MaxHeartRate = 180f; // beats per minute

    [SerializeField]
    private float LubDubGap = 0.15f;

    [SerializeField]
    private float HeartRateBuildupSpeed = 7f;

    [Header("Volume Settings")]
    [SerializeField]
    private AnimationCurve HeartbeatVolumeCurve = AnimationCurve.Linear(0, 0.5f, 1, 1f);
    
    [Header("Animation Settings")]
    [SerializeField]
    private float MinHeartIntensity = 1.05f;
    
    [SerializeField]
    private float MaxHeartIntensity = 1.3f;
    
    [SerializeField]
    private Color NormalHeartColor = Color.white;
    
    [SerializeField]
    private Color IntenseHeartColor = new Color(1f, 0.7f, 0.7f);

    private Coroutine heartBeatRoutine;
    private AudioSource audioSource;
    private float targetHeartRate;
    private Sequence heartbeatAnimation;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        heartElement = playerUI.rootVisualElement.Q("heartbeat-representation");
    }

    public override void ActivateSymptom(float intensity)
    {
        enabled = true;
        targetHeartRate = Mathf.Lerp(MinHeartRate, MaxHeartRate, intensity);
        if (heartBeatRoutine == null)
        {
            heartBeatRoutine = StartCoroutine(HeartBeatRoutine());
        }
    }
    
    private void StartHeartbeatAnimation(float bpm, float intensity)
    {
        // Stop any existing animation
        if (heartbeatAnimation != null && heartbeatAnimation.IsActive())
        {
            heartbeatAnimation.Kill();
        }
        
        // Create a new realistic heartbeat animation
        heartbeatAnimation = heartElement.DORealisticHeartbeat(
            bpm,
            MinHeartRate,
            MaxHeartRate
        );
        
        // Add color pulsing based on intensity
        Color targetColor = Color.Lerp(NormalHeartColor, IntenseHeartColor, intensity);
        
        // Add color animation to the heartbeat
        Sequence colorSequence = DOTween.Sequence();
        colorSequence.Append(heartElement.DOColor(targetColor, 60f / bpm * 0.15f));
        colorSequence.Append(heartElement.DOColor(NormalHeartColor, 60f / bpm * 0.85f));
        colorSequence.SetLoops(-1);
    }

    private IEnumerator HeartBeatRoutine()
    {
        float currentHeartRate = MinHeartRate;
        while (enabled)
        {
            float volume = HeartbeatVolumeCurve.Evaluate(Intensity);
            currentHeartRate = Mathf.Lerp(
                currentHeartRate,
                targetHeartRate,
                Time.deltaTime * HeartRateBuildupSpeed
            );

            audioSource.PlayOneShot(HeartBeatLub, volume);
            yield return new WaitForSeconds(LubDubGap);

            audioSource.PlayOneShot(HeartBeatDub, volume);
            
            if (Mathf.Abs(currentHeartRate - targetHeartRate) < 5f && 
                heartbeatAnimation != null && 
                !heartbeatAnimation.IsPlaying())
            {
                StartHeartbeatAnimation(currentHeartRate, Intensity);
            }
            
            yield return new WaitForSeconds((60f / currentHeartRate) - LubDubGap);
        }

        heartBeatRoutine = null;
    }

    public override void StopSymptom()
    {
        enabled = false;
        
        // Stop audio routine
        if (heartBeatRoutine != null)
        {
            StopCoroutine(heartBeatRoutine);
            heartBeatRoutine = null;
        }
        
        // Stop visual animation
        if (heartbeatAnimation != null && heartbeatAnimation.IsActive())
        {
            // Create a smooth fade-out effect
            heartbeatAnimation.Complete();
            heartbeatAnimation.Kill();
            
            if (heartElement != null)
            {
                // Smoothly return to normal
                heartElement.DOScale(1f, 0.5f);
                heartElement.DOColor(NormalHeartColor, 0.5f);
            }
        }
    }
}
