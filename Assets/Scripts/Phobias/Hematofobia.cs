using UnityEngine;

public class Hematophobia : MentalIllness
{
    [SerializeField] private float maxRayDistance = 5f;
    [SerializeField] private LayerMask bloodMask;

    private Camera playerCamera;

    private void Start()
    {
        // Assign symptoms to the player
        RequireSymptom<VisualDistortion>();
        RequireSymptom<Trembling>();
        RequireSymptom<HeartBeat>();
        RequireSymptom<Breathing>();

        playerCamera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        CheckIfLookingAtBlood();
        HandleAnxiety();
    }

    /// <summary>
    /// Casts a ray to check if the player is looking at blood and updates anxiety accordingly.
    /// </summary>
    private void CheckIfLookingAtBlood()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, bloodMask, QueryTriggerInteraction.Collide))
        {
            PendNewAnxietyLevel(1f);
        }
    }
}