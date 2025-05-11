using System;
using UnityEngine;

public abstract class MemoryTrigger : MonoBehaviour
{

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