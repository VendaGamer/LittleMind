using UnityEngine;
[CreateAssetMenu(menuName = "InteractableInfo/DoorInfo",fileName = "DoorInfo")]
public class DoorInfo : InteractableInfoBase
{
    [field:SerializeField] public float LerpDuration { get; private set; }
    [field:SerializeField] public float OpenAngle { get; private set; }
    [field:SerializeField] public Interaction OpenDoorInteraction { get; private set; }
    [field:SerializeField] public Interaction CloseDoorInteraction { get; private set; }
    [field:SerializeField] public Interaction LookThroughKeyHoleInteraction { get; private set; }
}