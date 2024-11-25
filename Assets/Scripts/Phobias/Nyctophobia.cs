using UnityEngine;

public class Nyctophobia : MentalIllness
{

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
        if (isInLight)
        {
            RecoverAnxietyLevel();
        }
        else
        {
            CurrentAnxietyLevel = Mathf.Min(
                CurrentAnxietyLevel + (anxietyBuildUpAndRecoveryRate * Time.fixedDeltaTime),
                maxAnxietyLevel
            );
            
            UpdateSymptoms();
        }
        isInLight = false; // Reset for the next frame
    }
    

    /// <summary>
    /// Snizuje uroven uzkosti, pokud je hrac ve svetle
    /// </summary>
    private void RecoverAnxietyLevel()
    {
        CurrentAnxietyLevel = Mathf.Max(CurrentAnxietyLevel - (anxietyBuildUpAndRecoveryRate * Time.fixedDeltaTime), 0f);

        if (CurrentAnxietyLevel <= 0f)
        {
            RecoverFromSymptoms();
        }
    }

    /// <summary>
    /// Zavolano, kdyz hrac vstoupi do svetla, signalizuje uzdravovani
    /// </summary>
    public void RecoverAnxiety()
    {
        isInLight = true;
    }
}