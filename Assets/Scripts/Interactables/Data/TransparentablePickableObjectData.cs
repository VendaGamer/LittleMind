using UnityEngine;
[CreateAssetMenu(menuName = "InteractableInfo/TransparentablePickableObjectInfo",fileName = "TransparentablePickableObjectInfo")]
public class TransparentablePickableObjectData : PickableObjectData
{
    [field:SerializeField, Range(0,1f)] public float TransparencyPercentage { get; private set; }
}