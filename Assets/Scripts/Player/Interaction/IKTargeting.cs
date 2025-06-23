using UnityEngine;
using UnityEngine.Animations.Rigging;

public partial class PlayerController
{
    [Header("Hand Settings")]
    [SerializeField]
    private TwoBoneIKConstraint leftHandIKConstraint;
    
    [SerializeField]
    private TwoBoneIKConstraint rightHandIKConstraint;

    
    [Header("Leg Settings")]
    [SerializeField]
    private TwoBoneIKConstraint leftLegIKConstraint;
    
    [SerializeField]
    private TwoBoneIKConstraint rightLegIKConstraint;
    
    [SerializeField]
    private RigBuilder rigBuilder;

    private IKTargetType currentLock;
    

    private void LockRightHandTo(Transform target)
    {
        if (!target)
            return;
        rightHandIKConstraint.data.target = target;
        rightHandIKConstraint.weight = 1f;
        rigBuilder.Build();
        animator.Rebind();
    }

    private void UnlockRightHand()
    {
        if (currentLock == IKTargetType.none)
            return;
        rightHandIKConstraint.weight = 0f;
        rightHandIKConstraint.data.target = null;
        rigBuilder.Build();
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

    public void SetIKTarget(IKTargetType targetType)
    {
        currentLock = targetType;
        switch (targetType)
        {
            case IKTargetType.key:
                
                break;
            case IKTargetType.twoHand:

                break;
            default:
                break;
        }
    }
}
