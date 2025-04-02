using UnityEngine;
[CreateAssetMenu(menuName = "GameData/PickableObjectData",fileName = "PickableObjectData")]
public class PickableObjectData : InteractableDataBase
{
    [field:SerializeField] public float LerpDuration { get; private set; }
    [field:SerializeField] public Interaction PickupInteraction { get; private set; }
    [field:SerializeField] public Interaction DropInteraction { get; private set; }
}