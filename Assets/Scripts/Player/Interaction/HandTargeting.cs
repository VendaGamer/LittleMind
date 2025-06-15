using UnityEngine;
using UnityEngine.Animations.Rigging;

public partial class PlayerController
{
    [Header("Hand Settings")]
    [SerializeField]
    private TwoBoneIKConstraint leftHandIKConstraint,
        rightHandIKConstraint;

    [SerializeField]
    Transform rightHandIKConstraintTransform,
        leftHandIKConstraintTransform;

    [SerializeField]
    private RigBuilder handRigBuilder;

    [Header("Hand Targets")]
    [SerializeField]
    private Transform rightHandKeyLockTarget;

    private HandTargetType currentLock;

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
        if (currentLock == HandTargetType.none)
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
        if (currentLock == HandTargetType.none)
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
            case HandTargetType.key:
                UnlockRightHand();
                break;
            case HandTargetType.twoHand:

                break;
            default:
                break;
        }

        currentLock = HandTargetType.none;
    }

    public void SetHandTarget(HandTargetType targetType)
    {
        currentLock = targetType;
        switch (targetType)
        {
            case HandTargetType.key:
                LockRightHandTo(rightHandKeyLockTarget);
                break;
            case HandTargetType.twoHand:

                break;
            default:
                break;
        }
    }
}
