using UnityEngine;

public class Akrofobia : MentalIllness
{
    [SerializeField] private float maxCheckDistance = 6f;

    private void Start()
    {
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

    public void SetAnxietyBasedOnDistance(float distance)
    {
        if(distance > maxCheckDistance)return;
            PendNewAnxietyLevel(Mathf.Lerp(1,0,distance / maxCheckDistance));
    }
}
