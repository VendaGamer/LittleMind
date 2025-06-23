using UnityEngine;

public abstract class InteractableData : ScriptableObject
{
    [field:SerializeField] public string InteractableGroupLabel { get; private set; }
}