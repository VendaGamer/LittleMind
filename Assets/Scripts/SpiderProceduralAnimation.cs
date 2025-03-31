using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderProceduralAnimation : MonoBehaviour
{
    [System.Serializable]
    private class LegInfo
    {
        public Transform footTarget;    // IK target for the foot
        public Transform raycastOrigin; // Point to cast ray from to find ground
        public bool isMoving = false;   // Whether leg is currently taking a step
        public Vector3 defaultPosition; // Default position relative to body
        public Vector3 lastPosition;    // Last grounded position
    }
    
    [SerializeField] private LegInfo[] legs;
    [SerializeField] private float moveThreshold = 1.5f;    // Distance before taking a step
    [SerializeField] private float stepDuration = 0.5f;     // How long each step takes
    [SerializeField] private float raycastDistance = 2f;    // How far to check for ground
    [SerializeField] private LayerMask groundLayer;         // Layer for ground detection
    [SerializeField] private float bodyHeight = 1.5f;       // Desired height above ground
    [SerializeField] private float stepHeight = 0.5f; // Height of leg lifting during step
    [SerializeField] public float footSpacing = 1f;
    private Dictionary<LegInfo, Coroutine> activeSteps = new Dictionary<LegInfo, Coroutine>();

    void Start()
    {
        // Initialize default positions
        foreach (var leg in legs)
        {
            leg.defaultPosition = leg.footTarget.localPosition;
            leg.lastPosition = leg.footTarget.position;
        }
    }

    void Update()
    {
        UpdateLegs();
        UpdateBody();
    }

    void UpdateLegs()
    {
        foreach (var leg in legs)
        {
            if (!leg.isMoving)
            {
                // Check if leg is too far from desired position
                float distanceFromDefault = Vector3.Distance(
                    transform.TransformPoint(leg.defaultPosition),
                    leg.footTarget.position
                );

                if (distanceFromDefault > moveThreshold)
                {
                    // Start a new step if leg isn't already moving
                    if (!activeSteps.ContainsKey(leg))
                    {
                        var stepCoroutine = StartCoroutine(TakeStep(leg));
                        activeSteps[leg] = stepCoroutine;
                    }
                }
            }
        }
    }

    void UpdateBody()
    {
        // Calculate average ground height from all legs
        float totalHeight = 0f;
        int legCount = 0;

        foreach (var leg in legs)
        {
            RaycastHit hit;
            if (Physics.Raycast(leg.raycastOrigin.position, Vector3.down, out hit, raycastDistance, groundLayer))
            {
                totalHeight += hit.point.y;
                legCount++;
            }
        }

        if (legCount > 0)
        {
            float targetHeight = (totalHeight / legCount) + bodyHeight;
            Vector3 newPos = transform.position;
            newPos.y = Mathf.Lerp(transform.position.y, targetHeight, Time.deltaTime * 5f);
            transform.position = newPos;
        }
    }

    System.Collections.IEnumerator TakeStep(LegInfo leg)
    {
        leg.isMoving = true;
        Vector3 startPos = leg.footTarget.position;
        
        // Calculate target position
        Vector3 desiredPos = transform.TransformPoint(leg.defaultPosition);
        RaycastHit hit;
        if (Physics.Raycast(desiredPos + Vector3.up * raycastDistance, Vector3.down, out hit, raycastDistance * 2f, groundLayer))
        {
            desiredPos = hit.point;
        }

        float elapsed = 0f;
        while (elapsed < stepDuration)
        {
            float t = elapsed / stepDuration;
            
            // Calculate step trajectory
            float heightCurve = Mathf.Sin(t * Mathf.PI) * stepHeight;
            Vector3 currentPos = Vector3.Lerp(startPos, desiredPos, t);
            currentPos.y += heightCurve;
            
            leg.footTarget.position = currentPos;
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        leg.footTarget.position = desiredPos;
        leg.lastPosition = desiredPos;
        leg.isMoving = false;
        activeSteps.Remove(leg);
    }
}
