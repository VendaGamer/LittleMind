using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Breathing : AnxietySymptom
{
    [SerializeField] private AudioClip breatheIn;
    [SerializeField] private AudioClip breatheOut;
    [SerializeField] private AudioClip heavyBreatheIn;
    [SerializeField] private AudioClip heavyBreatheOut;
    [Header("Timing Settings")]
    [SerializeField] private float InOutGap = 0.15f;   // time between lub and dub
    [SerializeField] private float fadeDuration = 5f;
    [SerializeField] private float buildupSpeed = 7f;
    [Header("Volume Settings")]
    [SerializeField] private AnimationCurve heartbeatVolumeCurve = AnimationCurve.Linear(0, 0.5f, 1, 1f);

    private Coroutine currentBreatheRoutine;
    private AudioSource playerAudioSource;
    private float targetHeartRate;

    private void Start()
    {
        playerAudioSource = GetComponentInChildren<AudioSource>();
    }

    protected override void ActivateSymptom(float intensity)
    {

    }
}