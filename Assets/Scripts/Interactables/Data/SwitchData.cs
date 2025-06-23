using UnityEngine;
[CreateAssetMenu(menuName = "GameData/Interactables/SwitchData",fileName = "SwitchData")]
public class SwitchData : InteractableData
{
    [field: SerializeField] public Interaction switchOn;
    [field: SerializeField] public Interaction switchOff;
    [field: SerializeField] public float switchMoveDist = 0.04f;
    [field: SerializeField] public float switchMoveSpeed = 1f;
}