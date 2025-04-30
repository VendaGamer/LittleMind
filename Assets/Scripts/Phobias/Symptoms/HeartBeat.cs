using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class HeartBeatSymptom : AnxietySymptom
{
    [Header("UI")]
    [SerializeField]
    private UIDocument playerUI;

    private VisualElement heartElement;

    [FormerlySerializedAs("HeartBeatLub")]
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip heartBeatLub;

    [FormerlySerializedAs("HeartBeatDub")]
    [SerializeField]
    private AudioClip heartBeatDub;

    [FormerlySerializedAs("MinHeartRate")]
    [Header("Timing Settings")]
    [SerializeField]
    private float minHeartRate = 60f; // beats per minute

    [FormerlySerializedAs("MaxHeartRate")]
    [SerializeField]
    private float maxHeartRate = 180f; // beats per minute

    [FormerlySerializedAs("LubDubGap")]
    [SerializeField]
    private float lubDubGap = 0.15f;

    [FormerlySerializedAs("HeartRateBuildupSpeed")]
    [SerializeField]
    private float heartRateBuildupSpeed = 7f;

    [FormerlySerializedAs("HeartbeatVolumeCurve")]
    [Header("Volume Settings")]
    [SerializeField]
    private AnimationCurve heartbeatVolumeCurve = AnimationCurve.Linear(0, 0.5f, 1, 1f);

    [FormerlySerializedAs("MinHeartIntensity")]
    [Header("Animation Settings")]
    [SerializeField]
    private float minHeartIntensity = 1.05f;

    [FormerlySerializedAs("MaxHeartIntensity")]
    [SerializeField]
    private float maxHeartIntensity = 1.3f;

    [FormerlySerializedAs("NormalHeartColor")]
    [SerializeField]
    private Color normalHeartColor = Color.white;

    [FormerlySerializedAs("IntenseHeartColor")]
    [SerializeField]
    private Color intenseHeartColor = new Color(1f, 0.7f, 0.7f);

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
        targetHeartRate = Mathf.Lerp(minHeartRate, maxHeartRate, intensity);
        if (heartBeatRoutine == null)
        {
            heartBeatRoutine = StartCoroutine(HeartBeatRoutine());
        }
    }

    public override void OnAnxietyChanged(float newAnxiety)
    {
        base.OnAnxietyChanged(newAnxiety);
        targetHeartRate = Mathf.Lerp(minHeartRate, maxHeartRate, newAnxiety);
        if (heartBeatRoutine == null)
        {
            heartBeatRoutine = StartCoroutine(HeartBeatRoutine());
        }
    }

    private IEnumerator HeartBeatRoutine()
    {
        float currentHeartRate = minHeartRate;
        while (enabled)
        {
            float volume = heartbeatVolumeCurve.Evaluate(Intensity);
            currentHeartRate = Mathf.Lerp(
                currentHeartRate,
                targetHeartRate,
                Time.deltaTime * heartRateBuildupSpeed
            );

            audioSource.PlayOneShot(heartBeatLub, volume);
            heartElement.DOScale(1.2f, lubDubGap);
            yield return new WaitForSeconds(lubDubGap);

            audioSource.PlayOneShot(heartBeatDub, volume);
            var gap = (60f / currentHeartRate) - lubDubGap;
            heartElement.DOScale(1f, gap);

            yield return new WaitForSeconds(gap);
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
                heartElement.DOColor(normalHeartColor, 0.5f);
            }
        }
    }
}
