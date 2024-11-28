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
                CurrentAnxietyLevel + (anxietyBuildUpRate * Time.fixedDeltaTime),
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