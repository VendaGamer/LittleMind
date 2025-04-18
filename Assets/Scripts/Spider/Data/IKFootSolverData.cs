using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Spider/IKFootSolverData")]
public class IKFootSolverData : ScriptableObject
{
    [field:SerializeField] public LayerMask TerrainLayer { get; private set; } = default;
    [field:SerializeField] public float Speed { get; private set; } = 1f;
    [field:SerializeField] public float StepDistance { get; private set; } = 0.1f;
    [field:SerializeField] public float StepLength { get; private set; } = 0.1f;
    [field:SerializeField] public float StepHeight { get; private set; } = 0.1f;
    [field:SerializeField] public Vector3 FootOffset { get; private set; } = default;
    [field:SerializeField] public float MaxRaycastDistance { get; private set; } = 10f;
    
    // Added spider gait pattern controls
    [field:SerializeField] public float LegLiftDelay { get; private set; } = 0.1f;
    [field:SerializeField] public float GaitSpeed { get; private set; } = 1f;
}