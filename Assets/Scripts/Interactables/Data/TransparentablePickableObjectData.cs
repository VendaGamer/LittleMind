using UnityEngine;
[CreateAssetMenu(menuName = "GameData/Interactables/TransparentablePickableObject",fileName = "TransparentablePickableObject")]
public class TransparentablePickableObjectData : PickableObjectData
{
    [field:SerializeField, Range(0,1f)] public float TransparencyPercentage { get; private set; }
}