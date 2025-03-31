using UnityEngine;
[CreateAssetMenu(menuName = "InteractableInfo/UnlockableDoor",fileName = "UnlockableDoorInfo")]
public class UnlockableDoorData : DoorData
{
    [field:SerializeField] public Interaction UnlockDoorInteraction { get; private set; }
}