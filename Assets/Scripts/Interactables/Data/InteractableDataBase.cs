using UnityEngine;

public class InteractableDataBase : ScriptableObject
{
    [field:SerializeField] public string InteractableGroupLabel { get; private set; }
    [field:SerializeField, Range(0,10)] public int InteractablePriority { get; private set; }
}