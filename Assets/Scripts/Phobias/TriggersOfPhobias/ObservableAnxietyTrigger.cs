using System;
using UnityEngine;

/// <summary>
/// Takes care of Anxiety triggering on staying 
/// </summary>
/// <typeparam name="T">Phobia</typeparam>
public abstract class ObservableAnxietyTrigger<T> : MonoBehaviour where T : AnxietyManager
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<T>(out var anxietyManager))
        {
            OnPlayerWithAnxietyStay(anxietyManager);
        }
    }
    protected abstract void OnPlayerWithAnxietyStay(T other);
}