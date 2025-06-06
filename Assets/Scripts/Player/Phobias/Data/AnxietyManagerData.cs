using System;
using UnityEngine;
[CreateAssetMenu(menuName = "GameData/AnxietyManager",fileName = "AnxietyManagerData")]
public class AnxietyManagerData : ScriptableObject
{
    [field:SerializeField] public float MaxAnxietyLevel { get; protected set; } = 1f;
    [field:SerializeField] public float FadeDuration { get; protected set; } = 20f;
    [field:SerializeField] public int FadeOutDelayInMs { get; protected set; } = 5000;
}