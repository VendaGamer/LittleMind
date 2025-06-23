
using UnityEngine;

public interface ILadderTargets
{
    public Transform LeftHandTarget { get; }
    public Transform RightHandTarget { get; }
    public Transform LeftLegTarget { get; }
    public Transform RightLegTarget { get; }
}