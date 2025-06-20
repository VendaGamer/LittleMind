using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Configuration")]
    [SerializeField] 
    private uint numberOfSteps = 7;
    
    [SerializeField]
    private float ladderWidth = 0.8f;
    
    [SerializeField]
    private Transform topPointOfStep;
    
    [SerializeField]
    private Transform bottomPointOfStep;

    [SerializeField] 
    private float topBottomCrop = 0f;

    [SerializeField] 
    [Range(0f, 1f)]
    private float climbProgress = 0f;

    private Vector3[] targetPositions;
    
    [Header("Current Limb Positions")]
    public Vector3 leftHandPosition;
    public Vector3 rightHandPosition;
    public Vector3 leftLegPosition;
    public Vector3 rightLegPosition;

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {

    }
    
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
            Gizmos.DrawCube(leftHandPosition, Vector3.one * 0.08f);
            Gizmos.DrawCube(rightHandPosition, Vector3.one * 0.08f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(leftLegPosition, 0.06f);
            Gizmos.DrawSphere(rightLegPosition, 0.06f);
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
        
        climbProgress = Mathf.Clamp01(climbProgress);
        
        float progressSteps = climbProgress * numberOfSteps;
        int baseLegStep = Mathf.FloorToInt(progressSteps);
        int baseHandStep = Mathf.Min(baseLegStep + 2, targetPositions.Length - 1);
        
        // Determine which limbs are on which steps
        // The climbing pattern: at any point, 3 limbs are placed, 1 is moving
        
        if (baseLegStep % 2 == 0) // Even step - left leg is the base
        {
            leftLegPosition = GetStepPosition(baseLegStep);
            rightLegPosition = GetStepPosition(Mathf.Max(0, baseLegStep - 1));
            
            leftHandPosition = GetStepPosition(baseHandStep);
            rightHandPosition = GetStepPosition(Mathf.Max(0, baseHandStep - 1));
        }
        else // Odd step - right leg is the base
        {
            rightLegPosition = GetStepPosition(baseLegStep);
            leftLegPosition = GetStepPosition(Mathf.Max(0, baseLegStep - 1));
            
            rightHandPosition = GetStepPosition(baseHandStep);
            leftHandPosition = GetStepPosition(Mathf.Max(0, baseHandStep - 1));
        }
    }
    
    private Vector3 GetStepPosition(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= targetPositions.Length)
            return Vector3.zero;
            
        return targetPositions[stepIndex];
    }
    
    private void CalculateStepPositions()
    {
        targetPositions = new Vector3[numberOfSteps];

        float croppedBottomY = bottomPointOfStep.position.y + topBottomCrop;
        
        float interval = (topPointOfStep.position.y - topBottomCrop) - croppedBottomY;
        interval /= (numberOfSteps - 1);
        Vector3 ladderRight = transform.right;

        for (var i = 0; i < numberOfSteps; i++)
        {
            var targetY = croppedBottomY + (interval * i);
            
            Vector3 centerPosition = new Vector3(
                bottomPointOfStep.position.x, 
                targetY, 
                bottomPointOfStep.position.z
            );
            
            if (i % 2 == 0)
            {
                targetPositions[i] = centerPosition - ladderRight * (ladderWidth / 2);
            }
            else
            {
                targetPositions[i] = centerPosition + ladderRight * (ladderWidth / 2);
            }
        }
    }
}