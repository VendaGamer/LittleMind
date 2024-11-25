using UnityEngine;

public class Nyctophobia : MentalIllness
{
    [SerializeField] private float anxietyRecoverySpeed = 1.5f;
    [SerializeField] private float anxietyBuildUpSpeed = 0.05f;
    
    private bool isInLight = false;
    
    private void Start()
    {
        RequireSymptom<Trembling>();
        RequireSymptom<VisualDistortion>();
        RequireSymptom<HeartBeat>();
        RequireSymptom<Breathing>();
    }

    private void FixedUpdate()
    {

        if (!isInLight)
        {
            CurrentAnxietyLevel += anxietyBuildUpSpeed * Time.fixedDeltaTime;
            UpdateSymptoms();
        }
        
        isInLight = false;
    }

    public void RecoverAnxiety()
    {
        isInLight = true;
        RecoverFromSymptoms();
    }

    protected override void RecoverFromSymptoms()
    {
        
    }
}