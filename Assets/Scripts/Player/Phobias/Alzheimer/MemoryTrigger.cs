using System;
using UnityEngine;
using Pages = DiaryPage.Pages;
public abstract class MemoryTrigger : MonoBehaviour
{
    [field: SerializeField] public Pages page { get; private set; }
    public abstract Bounds BoundsToLookAt { get; }
    public abstract void MemoryDiscovered();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Alzheimer>(out var alzh))
        {
            alzh.RegisterMemoryTrigger(this);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Alzheimer>(out var alzh))
        {
            alzh.UnregisterMemoryTrigger(this);
        }
    }
}