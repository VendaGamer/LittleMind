using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class Acrofobia : AnxietyManager
{
    [Header("Acrophobia Settings")]
    [SerializeField] private float maxCheckDistance = 6f;
    [SerializeField] private float eyesCloseTriggerDelay = 5f;
    [SerializeField] private float eyesOpenDelay = 3f;
    [SerializeField] private RawImage eyeOverlay;

    private Coroutine eyesClosingRoutine;
    private Coroutine eyesOpeningRoutine;
    

    
    /// <summary>
    /// Simulates closing eyes by enabling the overlay after a delay.
    /// </summary>
    private IEnumerator CloseEyesAfterDelay()
    {
        yield return new WaitForSeconds(eyesCloseTriggerDelay);
        eyeOverlay.enabled = true;
    }

    /// <summary>
    /// Simulates opening eyes by disabling the overlay after a delay.
    /// </summary>
    private IEnumerator OpenEyesAfterDelay()
    {
        yield return new WaitForSeconds(eyesOpenDelay);
        eyeOverlay.enabled = false;
    }
}