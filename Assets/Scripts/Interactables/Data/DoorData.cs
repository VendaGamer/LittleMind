using UnityEngine;
[CreateAssetMenu(menuName = "GameData/DoorData",fileName = "DoorData")]
public class DoorData : InteractableDataBase
{
    [field:SerializeField] public float LerpDuration { get; private set; }
    [field:SerializeField] public float OpenAngle { get; private set; }
    [field:SerializeField] public Interaction OpenDoorInteraction { get; private set; }
    [field:SerializeField] public Interaction CloseDoorInteraction { get; private set; }
    [field:SerializeField] public Interaction LookThroughKeyHoleInteraction { get; private set; }
}