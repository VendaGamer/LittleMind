using UnityEngine;

public class InteractableInfoBase : ScriptableObject
{
    [field:SerializeField] public string InteractableGroupLabel { get; private set; }
}