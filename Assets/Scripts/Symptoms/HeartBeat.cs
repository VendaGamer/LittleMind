using System.Collections;
using UnityEngine;
public class HeartBeat : Symptom
{
    public override bool HasSelfManagedFadeOutAndFadeIn { get; protected set; } = true;
    [SerializeField] private AudioClip heartBeatLub;
    [SerializeField] private AudioClip heartBeatDub;

    [Header("Timing Settings")]
    [SerializeField] private float minHeartRate = 60f;  // beats per minute
    [SerializeField] private float maxHeartRate = 180f; // beats per minute
    [SerializeField] private float lubDubGap = 0.15f;   // time between lub and dub
    [SerializeField] private float fadeDuration = 5f;
    [SerializeField] private float heartRateBuildupSpeed = 7f;
    [Header("Volume Settings")]
    [SerializeField] private AnimationCurve heartbeatVolumeCurve = AnimationCurve.Linear(0, 0.5f, 1, 1f);

    private Coroutine currentHeartBeatRoutine;
    private AudioSource playerAudioSource;
    private float targetHeartRate;

    private void Start()
    {
        playerAudioSource = GetComponentInChildren<AudioSource>();
    }

    public override void UpdateOrTriggerSymptom(float intensity)
    {
        IsActive = true;
        Intensity = intensity;

        targetHeartRate = Mathf.Lerp(minHeartRate, maxHeartRate, intensity);
    
        currentHeartBeatRoutine ??= StartCoroutine(StartHeartBeat());
    }

    private IEnumerator StartHeartBeat()
    {
        float currentHeartRate = minHeartRate;
        while (true)
        {
            float volume = heartbeatVolumeCurve.Evaluate(Intensity);
            if (!IsActive)
            {
                float fadeStartTime = Time.time;
                float startHeartRate = currentHeartRate;

                while (Time.time - fadeStartTime < fadeDuration)
                {
                    float progress = (Time.time - fadeStartTime) / fadeDuration;
                    currentHeartRate = Mathf.Lerp(startHeartRate, minHeartRate, progress);
                    volume = heartbeatVolumeCurve.Evaluate(Intensity) * Mathf.Lerp(1f, 0f, progress);
                
                    if (IsActive)
                    {
                        break;
                    }

                    playerAudioSource.PlayOneShot(heartBeatLub, volume);
                    yield return new WaitForSeconds(lubDubGap);

                    playerAudioSource.PlayOneShot(heartBeatDub, volume);
                    yield return new WaitForSeconds((60f / currentHeartRate) - lubDubGap);
                }

                if (!IsActive)
                {
                    currentHeartRate = minHeartRate;
                    currentHeartBeatRoutine = null;
                    Intensity = 0f;
                    StopAllCoroutines();
                }
            }

            currentHeartRate = Mathf.Lerp(currentHeartRate, targetHeartRate, Time.deltaTime * heartRateBuildupSpeed);
            playerAudioSource.PlayOneShot(heartBeatLub, volume);
            yield return new WaitForSeconds(lubDubGap);

            playerAudioSource.PlayOneShot(heartBeatDub, volume);
            yield return new WaitForSeconds((60f / currentHeartRate) - lubDubGap);
        }
    }

    public override void StopSymptom()
    {
        IsActive = false;
    }
}