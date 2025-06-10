using UnityEngine;

[CreateAssetMenu(menuName = "GameData/AnxietyTrigger", fileName = "AnxietyTriggerData")]
public class AnxietyTriggerData : ScriptableObject
{
    [field: SerializeField]
    public float AnxietyAmount { get; protected set; } = .01f;

    [field: SerializeField]
    public float InitialAnxietyAmount { get; protected set; } = .1f;
}
