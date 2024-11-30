using UnityEngine;

public class Nyctophobia : MentalIllness
{

    private bool isInLight = false;
    private void FixedUpdate()
    {
        if (isInLight)
        {
            RecoverFromSymptoms();
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
    /// Zavolano, kdyz hrac vstoupi do svetla, signalizuje uzdravovani
    /// </summary>
    public void RecoverAnxiety()
    {
        isInLight = true;
    }
}