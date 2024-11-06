using UnityEngine;

public class Nyctophobia : MentalIllness
{
    [SerializeField] private float anxietyRecoverySpeed = 1.5f;
    [SerializeField] private float anxietyBuildUpSpeed = 0.05f;
    
    private bool isInLight;
    
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
            PendNewAnxietyLevel(CurrentAnxietyLevel + (anxietyBuildUpSpeed * Time.fixedDeltaTime));
            UpdateSymptoms();
        }
        
        isInLight = false;
    }

    public void RecoverAnxiety()
    {
        if (CurrentAnxietyLevel > 0f)
        {

            // Gradually decrease anxiety
            CurrentAnxietyLevel = Mathf.Max(0, CurrentAnxietyLevel - (anxietyRecoverySpeed * Time.fixedDeltaTime));
            UpdateSymptoms();
            isInLight = true;
        }
        else
        {
            RecoverFromSymptoms();
        }

    }
}