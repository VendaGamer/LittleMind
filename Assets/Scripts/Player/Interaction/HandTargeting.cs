using UnityEngine;
using UnityEngine.Animations.Rigging;

public partial class PlayerController
{
    [Header("Hand Settings")]
    
    [SerializeField]
    private TwoBoneIKConstraint leftHandIKConstraint, rightHandIKConstraint;
    
    [SerializeField]
    Transform rightHandIKConstraintTransform, leftHandIKConstraintTransform;

    [SerializeField] private RigBuilder handRigBuilder;

    public void LockLeftHandTo(Transform target)
    {
        leftHandIKConstraint.data.target = target;
        leftHandIKConstraint.weight = 1f;
        handRigBuilder.Build();
        animator.Rebind();
    }

    public void UnlockLeftHand()
    {
        leftHandIKConstraint.weight = 0f;
        leftHandIKConstraint.data.target = null;
        handRigBuilder.Build();
        animator.Rebind();
    }
    
    public void LockRightHandTo(Transform target)
    {
        rightHandIKConstraint.data.target = target;
        rightHandIKConstraint.weight = 1f;
        handRigBuilder.Build();
        animator.Rebind();
    }

    public void UnlockRightHand()
    {
        leftHandIKConstraint.weight = 0f;
        leftHandIKConstraint.data.target = null;
        handRigBuilder.Build();
        animator.Rebind();
    }


}