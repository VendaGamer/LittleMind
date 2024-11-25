using System.Collections;
using UnityEngine;

public class HeartBeat : Symptom
{
    [SerializeField] private AudioClip heartBeatLub;
    [SerializeField] private AudioClip heartBeatDub;

    [Header("Timing Settings")]
    [SerializeField] private float minHeartRate = 60f;  // beats per minute
    [SerializeField] private float maxHeartRate = 180f; // beats per minute
    [SerializeField] private float lubDubGap = 0.15f;   // time between lub and dub
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
        while (IsActive)
        {
            float volume = heartbeatVolumeCurve.Evaluate(Intensity);

            currentHeartRate = Mathf.Lerp(currentHeartRate, targetHeartRate, Time.deltaTime * heartRateBuildupSpeed);
            playerAudioSource.PlayOneShot(heartBeatLub, volume);
            yield return new WaitForSeconds(lubDubGap);

            playerAudioSource.PlayOneShot(heartBeatDub, volume);
            yield return new WaitForSeconds((60f / currentHeartRate) - lubDubGap);
        }

        currentHeartBeatRoutine = null;
    }

    public override void StopSymptom()
    {
        IsActive = false;
    }
}