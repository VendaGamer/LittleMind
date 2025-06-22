using UnityEngine;
using UnityEngine.Animations.Rigging;

public partial class PlayerController
{
    [Header("Hand Settings")]
    [SerializeField]
    private TwoBoneIKConstraint leftHandIKConstraint;
    
    [SerializeField]
    private TwoBoneIKConstraint rightHandIKConstraint;

    [SerializeField]
    Transform rightHandIKConstraintTransform,
        leftHandIKConstraintTransform;

    [SerializeField]
    private RigBuilder handRigBuilder;

    [Header("Hand Targets")]
    [SerializeField]
    private Transform rightHandKeyLockTarget;
    
    [SerializeField]
    private Transform leftHandKeyLockTarget;
    
    [Header("Leg Settings")]
    [SerializeField]
    private TwoBoneIKConstraint leftLegIKConstraint;
    
    [SerializeField]
    private TwoBoneIKConstraint rightLegIKConstraint;

    [SerializeField]
    Transform rightLegIKConstraintTransform,
        leftLegIKConstraintTransform;

    [SerializeField]
    private RigBuilder LegRigBuilder;

    [Header("Leg Targets")]
    [SerializeField]
    private Transform rightLegKeyLockTarget;
    
    [SerializeField]
    private Transform leftLegKeyLockTarget;

    private IKTargetType currentLock;

    private void LockLeftHandTo(Transform target)
    {
        if (!target)
            return;
        leftHandIKConstraint.data.target = target;
        leftHandIKConstraint.weight = 1f;
        handRigBuilder.Build();
        animator.Rebind();
    }

    private void UnlockLeftHand()
    {
        if (currentLock == IKTargetType.none)
            return;
        leftHandIKConstraint.weight = 0f;
        leftHandIKConstraint.data.target = null;
        handRigBuilder.Build();
        animator.Rebind();
    }

    private void LockRightHandTo(Transform target)
    {
        if (!target)
            return;
        rightHandIKConstraint.data.target = target;
        rightHandIKConstraint.weight = 1f;
        handRigBuilder.Build();
        animator.Rebind();
    }

    private void UnlockRightHand()
    {
        if (currentLock == IKTargetType.none)
            return;
        rightHandIKConstraint.weight = 0f;
        rightHandIKConstraint.data.target = null;
        handRigBuilder.Build();
        animator.Rebind();
    }

    private void OnInteractableDrop()
    {
        switch (currentLock)
        {
            case IKTargetType.key:
                UnlockRightHand();
                break;
            case IKTargetType.twoHand:

                break;
            default:
                break;
        }

        currentLock = IKTargetType.none;
    }

    public void SetHandTarget(IKTargetType targetType)
    {
        currentLock = targetType;
        switch (targetType)
        {
            case IKTargetType.key:
                LockRightHandTo(rightHandKeyLockTarget);
                break;
            case IKTargetType.twoHand:

                break;
            default:
                break;
        }
    }
}
