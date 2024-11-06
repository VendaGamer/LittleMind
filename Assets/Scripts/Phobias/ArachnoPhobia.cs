using UnityEngine;

public class ArachnoPhobia : MentalIllness
{
    [SerializeField] protected float triggerDistance = 7.5f;
    public float TriggerDistance => triggerDistance;
    private void Start()
    {
        // Dame hraci vsechny symptomy
        RequireSymptom<VisualDistortion>();
        RequireSymptom<Trembling>();
        RequireSymptom<HeartBeat>();
        RequireSymptom<Breathing>();
    }

    private void FixedUpdate()
    {
        if (CurrentAnxietyLevel > 0f)
        {
            UpdateSymptoms();
        }
        else
        {
            RecoverFromSymptoms();
        }
        CurrentAnxietyLevel = 0f;
    }
    public void PendNewAnxietyLevelBasedOnDistance(float distance)
    {
        if (distance < maxAnxietyLevel) return;
        float intensity = 1f - ((distance / triggerDistance) /2);
        PendNewAnxietyLevel(intensity);
    }
}