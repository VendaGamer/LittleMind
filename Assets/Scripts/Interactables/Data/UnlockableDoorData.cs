using UnityEngine;
[CreateAssetMenu(menuName = "GameData/Interactables/UnlockableDoor",fileName = "UnlockableDoorData")]
public class UnlockableDoorData : DoorData
{
    [field:SerializeField] public Interaction UnlockDoorInteraction { get; private set; }
}