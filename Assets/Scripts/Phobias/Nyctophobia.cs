using System.Collections;
using Symptoms;
using UnityEngine;
using UnityEngine.Serialization;

public class Nyctophobia : MentalIllness
{
    [SerializeField] private float lightDistanceTrigger = 6f;
    [SerializeField] private float anxietyRecoverySpeed = 1.5f;
    private void Start()
    {
        RequireSymptom<Trembling>();
        RequireSymptom<VisualDistortion>();
        RequireSymptom<HeartBeat>();
        RequireSymptom<Breathing>();
    }
    private void OnDisable()
    {
        RecoverFromSymptoms();
        StopAllCoroutines();
    }
    protected override void RecoverFromSymptoms()
    {
        
    }
}
