using UnityEngine;

public class GeneralPickableObject : PickableObject
{
    [SerializeField] PickableObjectData data;
    protected override PickableObjectData Data => data;
}