using UnityEngine;
[CreateAssetMenu(menuName = "GameData/TransparentablePickableObjectData",fileName = "TransparentablePickableObjectData")]
public class TransparentablePickableObjectData : PickableObjectData
{
    [field:SerializeField, Range(0,1f)] public float TransparencyPercentage { get; private set; }
}