using UnityEngine;
[CreateAssetMenu(menuName = "GameData/DrawerData",fileName = "DrawerData")]
public class DrawerData : InteractableDataBase
{
    [field:SerializeField] public float LerpDuration { get; private set; }
    [field:SerializeField] public float OpenX { get; private set; }
    [field:SerializeField] public Interaction OpenDrawerInteraction { get; private set; }
    [field:SerializeField] public Interaction CloseDrawerInteraction { get; private set; }
}