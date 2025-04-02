using UnityEngine;
[CreateAssetMenu(menuName = "GameData/UnlockableData",fileName = "UnlockableDoorData")]
public class UnlockableDoorData : DoorData
{
    [field:SerializeField] public Interaction UnlockDoorInteraction { get; private set; }
}