using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DynamicTwoBoneIKConstraint : TwoBoneIKConstraint
{
    private void LateUpdate()
    {
        data.tip.position = data.target.position;
        data.tip.rotation = data.target.rotation;
    }
}