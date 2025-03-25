using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArachnoPhobia : AnxietyManager
{
    [Header("Acrophobia Settings")]
    [SerializeField] private float MaxCheckDistance = 6f;
    [SerializeField] private float EyesCloseTriggerDelay = 5f;
    [SerializeField] private float EyesOpenDelay = 3f;
    [SerializeField] private RawImage EyeOverlay;

    private Coroutine eyesRoutine;

    protected override void Start()
    {
        base.Start();
        AnxietyChanged += HandleAnxietyChanged;
    }

    private void OnDestroy()
    {
        AnxietyChanged -= HandleAnxietyChanged;
    }

    /// <summary>
    /// Stara se o 
    /// </summary>
    private void HandleAnxietyChanged(float anxiety)
    {
        if (anxiety > MaxAnxietyLevel - 0.1f)
        {
            if (eyesRoutine != null)
            {
                StopCoroutine(eyesRoutine);
            }
            eyesRoutine = StartCoroutine(CloseEyesAfterDelay());
        }
        else
        {
            if (eyesRoutine != null)
            {
                StopCoroutine(eyesRoutine);
            }
            eyesRoutine = StartCoroutine(OpenEyesAfterDelay());
        }
    }

    private IEnumerator CloseEyesAfterDelay()
    {
        yield return new WaitForSeconds(EyesCloseTriggerDelay);
        EyeOverlay.enabled = true;
    }

    private IEnumerator OpenEyesAfterDelay()
    {
        yield return new WaitForSeconds(EyesOpenDelay);
        EyeOverlay.enabled = false;
    }

}