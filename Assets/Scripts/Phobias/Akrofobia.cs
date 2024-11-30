using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Akrofobia : MentalIllness
{
    [SerializeField] private float maxCheckDistance = 6f;
    [SerializeField] private float eyesCloseTriggerDelay = 5f; // Delay before turning on the overlay
    [SerializeField] private float eyesOpenDelay = 3f; // Delay before turning off the overlay
    [SerializeField] private RawImage eyeClose; // Overlay for simulating closed eyes

    private Coroutine eyesClosingRoutine;
    private Coroutine eyesOpeningRoutine;

    private void FixedUpdate()
    {
        if (CurrentAnxietyLevel > maxAnxietyLevel - 0.1f)
        {
            if (eyesOpeningRoutine != null)
            {
                StopCoroutine(eyesOpeningRoutine); // Stop any ongoing opening routine
                eyesOpeningRoutine = null;
            }
            eyesClosingRoutine ??= StartCoroutine(CloseEyesAfterDelay());
        }
        else
        {
            if (eyesClosingRoutine != null)
            {
                StopCoroutine(eyesClosingRoutine); // Stop any ongoing closing routine
                eyesClosingRoutine = null;
            }
            eyesOpeningRoutine ??= StartCoroutine(OpenEyesAfterDelay());
        }

        HandleAnxiety();
    }

    /// <summary>
    /// Updates anxiety level based on distance to the trigger point.
    /// </summary>
    public void SetAnxietyBasedOnDistance(float distance)
    {
        if (distance > maxCheckDistance) return;

        PendNewAnxietyLevel(Mathf.Lerp(maxAnxietyLevel, 0f, distance / maxCheckDistance));
    }

    /// <summary>
    /// Simulates closing eyes by enabling the overlay after a delay.
    /// </summary>
    private IEnumerator CloseEyesAfterDelay()
    {
        yield return new WaitForSeconds(eyesCloseTriggerDelay);
        eyeClose.enabled = true;
    }

    /// <summary>
    /// Simulates opening eyes by disabling the overlay after a delay.
    /// </summary>
    private IEnumerator OpenEyesAfterDelay()
    {
        yield return new WaitForSeconds(eyesOpenDelay);
        eyeClose.enabled = false;
    }
}
