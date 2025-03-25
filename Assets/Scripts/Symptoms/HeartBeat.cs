using System.Collections;
using UnityEngine;

public class HeartBeatSymptom : AnxietySymptom
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip HeartBeatLub;
    [SerializeField] private AudioClip HeartBeatDub;

    [Header("Timing Settings")]
    [SerializeField] private float MinHeartRate = 60f;  // beats per minute
    [SerializeField] private float MaxHeartRate = 180f; // beats per minute
    [SerializeField] private float LubDubGap = 0.15f;
    [SerializeField] private float HeartRateBuildupSpeed = 7f;

    [Header("Volume Settings")]
    [SerializeField] private AnimationCurve HeartbeatVolumeCurve = AnimationCurve.Linear(0, 0.5f, 1, 1f);

    private Coroutine heartBeatRoutine;
    private AudioSource audioSource;
    private float targetHeartRate;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected override void ActivateSymptom(float intensity)
    {
        targetHeartRate = Mathf.Lerp(MinHeartRate, MaxHeartRate, intensity);
        if (heartBeatRoutine == null)
        {
            heartBeatRoutine = StartCoroutine(HeartBeatRoutine());
        }
    }

    private IEnumerator HeartBeatRoutine()
    {
        float currentHeartRate = MinHeartRate;
        while (enabled)
        {
            float volume = HeartbeatVolumeCurve.Evaluate(Intensity);
            currentHeartRate = Mathf.Lerp(currentHeartRate, targetHeartRate, Time.deltaTime * HeartRateBuildupSpeed);

            audioSource.PlayOneShot(HeartBeatLub, volume);
            yield return new WaitForSeconds(LubDubGap);

            audioSource.PlayOneShot(HeartBeatDub, volume);
            yield return new WaitForSeconds((60f / currentHeartRate) - LubDubGap);
        }

        heartBeatRoutine = null;
    }

    public override void StopSymptom()
    {
        enabled = false;
        if (heartBeatRoutine != null)
        {
            StopCoroutine(heartBeatRoutine);
            heartBeatRoutine = null;
        }
    }
}
