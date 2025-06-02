using UnityEngine;

public abstract class InteractableData : ScriptableObject
{
    [field:SerializeField] public string InteractableGroupLabel { get; private set; }
    [field:SerializeField, Range(0,10)] public int InteractablePriority { get; private set; }
}