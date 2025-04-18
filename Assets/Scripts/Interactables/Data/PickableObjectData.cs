using UnityEngine;
[CreateAssetMenu(menuName = "InteractableInfo/PickableObjectInfo",fileName = "PickableObjectInfo")]
public class PickableObjectData : InteractableData
{
    [field:SerializeField] public float LerpDuration { get; private set; }
    [field:SerializeField] public Interaction PickupInteraction { get; private set; }
    [field:SerializeField] public Interaction DropInteraction { get; private set; }
}