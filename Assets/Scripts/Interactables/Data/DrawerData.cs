using UnityEngine;
[CreateAssetMenu(menuName = "GameData/Interactables/DrawerData",fileName = "DrawerData")]
public class DrawerData : InteractableData
{
    [field:SerializeField] public float LerpDuration { get; private set; }
    [field:SerializeField] public float OpenX { get; private set; }
    [field:SerializeField] public Interaction OpenDrawerInteraction { get; private set; }
    [field:SerializeField] public Interaction CloseDrawerInteraction { get; private set; }
}