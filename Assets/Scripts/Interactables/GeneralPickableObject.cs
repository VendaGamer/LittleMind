using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class GeneralPickableObject : PickableObject
{
    [SerializeField] PickableObjectData data;
    protected override PickableObjectData Data => data;
    protected override ReadOnlyArray<Interaction> AllInteractions { get; }
}