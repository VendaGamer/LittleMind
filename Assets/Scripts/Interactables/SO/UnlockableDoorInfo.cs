using UnityEngine;
[CreateAssetMenu(menuName = "InteractableInfo/UnlockableDoor",fileName = "UnlockableDoorInfo")]
public class UnlockableDoorInfo : DoorInfo
{
    [field:SerializeField] public Interaction UnlockDoorInteraction { get; private set; }
}