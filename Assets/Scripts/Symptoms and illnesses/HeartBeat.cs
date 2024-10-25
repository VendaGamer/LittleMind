using System.Collections;
using UnityEngine;
public class HeartBeat : Symptom
{
    [SerializeField] private AudioClip lightHeartBeating;
    [SerializeField] private AudioClip mediumHeartBeating;
    [SerializeField] private AudioClip heavyHeartBeating;
    private float minBeatSpeed = 0.2f;
    private float maxBeatSpeed = 4f;
    private Coroutine currentHeartBeatRoutine;
    private AudioSource playerAudioSource;

    private void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
    }
    public override void UpdateOrTriggerSymptom(float intensity)
    {
        Intensity = intensity;
        if(currentHeartBeatRoutine == null)
        {
            currentHeartBeatRoutine = StartCoroutine(StartHeartBeat());
        }
    }
    private IEnumerator StartHeartBeat()
    {
        var clip = Intensity switch
        {
            < 0.3f => lightHeartBeating,
            < 0.6f => mediumHeartBeating,
            _ => heavyHeartBeating
        };
        Debug.Log("Beat");
        playerAudioSource.clip = clip;
        playerAudioSource.Play();
        yield return new WaitForSeconds(
            Mathf.Min(maxBeatSpeed,
                Mathf.Max(minBeatSpeed, minBeatSpeed / (1 + Intensity))
                )
            );

    }
    public override void StopSymptom()
    {
        base.StopSymptom();
        StopAllCoroutines();
        currentHeartBeatRoutine = null;
    }
}
