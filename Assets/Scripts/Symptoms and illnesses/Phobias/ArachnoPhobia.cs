using UnityEngine;

public class ArachnoPhobia : MentalIllness
{
    [SerializeField] private LayerMask spiderLayer;
    [SerializeField] protected float triggerDistance = 7.5f;
    public float TriggerDistance => triggerDistance;
    private void Start()
    {
        // Dame hraci vsechny symptomy
        RequireSymptom<VisualDistortion>();
        RequireSymptom<Trembling>();
        RequireSymptom<HeartBeat>();
    }
    public void PendNewAnxietyLevelBaseOnDistance(float distance)
    {
        if (distance < maxAnxietyLevel) return;
        float intensity = 1f - ((distance / triggerDistance) /2);
        PendNewAnxietyLevel(intensity);
    }
}