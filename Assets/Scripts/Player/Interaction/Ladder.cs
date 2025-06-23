using System;
using UnityEngine;

public class Ladder : MonoBehaviour, ILadderTargets
{
    [Header("Ladder Configuration")]
    [SerializeField] 
    private int numberOfSteps = 7;
    
    [SerializeField]
    private float ladderWidth = 0.8f;
    
    [SerializeField]
    private float targetPositionsOffset = 0f;
    
    [SerializeField]
    private Transform topPointOfStep;
    
    [SerializeField]
    private Transform bottomPointOfStep;

    [SerializeField] 
    private float topBottomCrop = 0f;

    [SerializeField] 
    private float climbProgress = 0f;

    private Vector3[] targetPositions;
    
    [Header("Positions of Targets of limbs")]
    [Tooltip("Determines destination of limb")]
    private int leftHandTargetDestinationIndex;
    private int rightHandTargetDestinationIndex;
    private int leftLegTargetDestinationIndex;
    private int rightLegTargetDestinationIndex;

    [Header("Current index of targets base")]
    [Tooltip("Determines position from which targets should lerp")]
    private int leftHandTargetCurrentIndex;
    private int rightHandTargetCurrentIndex;
    private int leftLegTargetCurrentIndex;
    private int rightLegTargetCurrentIndex;
    
    [Header("Targets of the limbs")]
    [Tooltip("IK constraints should be locked to them when climbing")]
    [SerializeField]
    private Transform leftHandTarget;
    [SerializeField]
    private Transform rightHandTarget;
    [SerializeField]
    private Transform leftLegTarget;
    [SerializeField]
    private Transform rightLegTarget;
    
    public Transform LeftHandTarget => leftHandTarget;
    public Transform RightHandTarget => rightHandTarget;
    public Transform LeftLegTarget => leftLegTarget;
    public Transform RightLegTarget => rightLegTarget;

    [SerializeField] 
    private float playerBodyOffset = 1f;

    [SerializeField]
    private AnimationCurve limbMovement;
    
    
    private void OnDrawGizmosSelected()
    {
        if (targetPositions != null)
        {
            // Draw all step positions
            foreach (var pos in targetPositions)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(pos, 0.05f);
            }
            
            // Draw current limb positions
            Gizmos.color = Color.green;
            Gizmos.DrawCube(targetPositions[leftHandTargetDestinationIndex],Vector3.one * 0.08f);
            Gizmos.DrawCube(targetPositions[rightHandTargetDestinationIndex], Vector3.one * 0.08f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(targetPositions[leftLegTargetDestinationIndex], 0.06f);
            Gizmos.DrawSphere(targetPositions[rightLegTargetDestinationIndex], 0.06f);
            
            Gizmos.color = Color.orangeRed;
            Gizmos.DrawCube(leftLegTarget.position, Vector3.one * 0.08f);
            Gizmos.DrawCube(rightLegTarget.position, Vector3.one * 0.08f);
            
            Gizmos.color = Color.dodgerBlue;
            Gizmos.DrawCube(leftHandTarget.position, Vector3.one * 0.08f);
            Gizmos.DrawCube(rightHandTarget.position, Vector3.one * 0.08f);
        }
    }
    
    private void OnValidate()
    {
        if (topPointOfStep && bottomPointOfStep)
        {
            CalculateStepPositions();
            CalculateStepProgress();
        }
    }


    private void CalculateStepProgress()
    {
        if (targetPositions == null || targetPositions.Length == 0)
            return;
        
        // Clamp progress to valid range (0 to numberOfSteps-1)
        climbProgress = Mathf.Clamp(climbProgress, 0f, numberOfSteps - 1);
        
        // Calculate current step and fraction within that step
        int currentStep = Mathf.FloorToInt(climbProgress);
        float stepFraction = climbProgress - currentStep; // 0-1 within current step
        
        // Calculate base positions for limbs (3-step offset pattern)
        int legStep = currentStep;
        int handStep = Mathf.Min(currentStep + 3, numberOfSteps - 1);
        
        // Determine climbing pattern based on current step
        if (currentStep % 2 == 0) // Even step - left side leads
        {
            // Calculate destination indices
            leftLegTargetDestinationIndex = Mathf.Min(legStep, numberOfSteps - 1);
            rightLegTargetDestinationIndex = Mathf.Max(0, legStep - 1);
            leftHandTargetDestinationIndex = Mathf.Min(handStep, numberOfSteps - 1);
            rightHandTargetDestinationIndex = Mathf.Max(0, handStep - 1);
            
            if (stepFraction < 0.5f) // First half - right leg moves
            {
                // Static positions
                leftLegTarget.position = targetPositions[leftLegTargetDestinationIndex];
                leftHandTarget.position = targetPositions[leftHandTargetDestinationIndex];
                rightHandTarget.position = targetPositions[rightHandTargetDestinationIndex];
                
                // Moving limb - right leg
                rightLegTargetCurrentIndex = Mathf.Max(0, legStep - 2);
                float moveProgress = stepFraction * 2f; // Scale 0-0.5 to 0-1
                rightLegTarget.position = Vector3.Lerp(
                    targetPositions[rightLegTargetCurrentIndex], 
                    targetPositions[rightLegTargetDestinationIndex], 
                    moveProgress
                );
            }
            else // Second half - left hand moves
            {
                // Static positions
                leftLegTarget.position = targetPositions[leftLegTargetDestinationIndex];
                rightLegTarget.position = targetPositions[rightLegTargetDestinationIndex];
                rightHandTarget.position = targetPositions[rightHandTargetDestinationIndex];
                
                // Moving limb - left hand
                leftHandTargetCurrentIndex = Mathf.Max(0, handStep - 1);
                float moveProgress = (stepFraction - 0.5f) * 2f; // Scale 0.5-1 to 0-1
                leftHandTarget.position = Vector3.Lerp(
                    targetPositions[leftHandTargetCurrentIndex], 
                    targetPositions[leftHandTargetDestinationIndex], 
                    moveProgress
                );
            }
        }
        else // Odd step - right side leads
        {
            // Calculate destination indices
            rightLegTargetDestinationIndex = Mathf.Min(legStep, numberOfSteps - 1);
            leftLegTargetDestinationIndex = Mathf.Max(0, legStep - 1);
            rightHandTargetDestinationIndex = Mathf.Min(handStep, numberOfSteps - 1);
            leftHandTargetDestinationIndex = Mathf.Max(0, handStep - 1);
            
            if (stepFraction < 0.5f) // First half - left leg moves
            {
                // Static positions
                rightLegTarget.position = targetPositions[rightLegTargetDestinationIndex];
                leftHandTarget.position = targetPositions[leftHandTargetDestinationIndex];
                rightHandTarget.position = targetPositions[rightHandTargetDestinationIndex];
                
                // Moving limb - left leg
                leftLegTargetCurrentIndex = Mathf.Max(0, legStep - 2);
                float moveProgress = stepFraction * 2f; // Scale 0-0.5 to 0-1
                leftLegTarget.position = Vector3.Lerp(
                    targetPositions[leftLegTargetCurrentIndex], 
                    targetPositions[leftLegTargetDestinationIndex], 
                    moveProgress
                );
            }
            else // Second half - right hand moves
            {
                // Static positions
                rightLegTarget.position = targetPositions[rightLegTargetDestinationIndex];
                leftLegTarget.position = targetPositions[leftLegTargetDestinationIndex];
                leftHandTarget.position = targetPositions[leftHandTargetDestinationIndex];
                
                // Moving limb - right hand
                rightHandTargetCurrentIndex = Mathf.Max(0, handStep - 1);
                float moveProgress = (stepFraction - 0.5f) * 2f; // Scale 0.5-1 to 0-1
                rightHandTarget.position = Vector3.Lerp(
                    targetPositions[rightHandTargetCurrentIndex], 
                    targetPositions[rightHandTargetDestinationIndex], 
                    moveProgress
                );
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var PlayerController))
        {
            Debug.Log($"PlayerController: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
    
    private void CalculateStepPositions()
    {
        targetPositions = new Vector3[numberOfSteps];

        // Get the actual direction vector of the ladder
        Vector3 ladderDirection = (topPointOfStep.position - bottomPointOfStep.position).normalized;
    
        // Calculate cropped bottom and top positions
        Vector3 croppedBottomPos = bottomPointOfStep.position + ladderDirection * topBottomCrop;
        Vector3 croppedTopPos = topPointOfStep.position - ladderDirection * topBottomCrop;
    
        // Get the ladder's right direction for alternating step positions
        Vector3 ladderRight = transform.right;

        for (var i = 0; i < numberOfSteps; i++)
        {
            // Interpolate along the ladder's direction
            float t = (float)i / (numberOfSteps - 1);
            Vector3 centerPosition = Vector3.Lerp(croppedBottomPos, croppedTopPos, t);
        
            // Alternate sides based on step index
            if (i % 2 == 0)
            {
                targetPositions[i] = centerPosition - ladderRight * (ladderWidth / 2);
            }
            else
            {
                targetPositions[i] = centerPosition + ladderRight * (ladderWidth / 2);
            }
            targetPositions[i] += transform.forward * targetPositionsOffset;
        }
    }
}