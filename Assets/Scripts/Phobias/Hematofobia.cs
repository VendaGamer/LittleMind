using System;
using UnityEngine;

public class Hematofobia : MentalIllness
{
    private Camera playerCamera;
    [SerializeField]
    private float maxRayDist = 5f;
    [SerializeField]
    private LayerMask bloodMask;
    private void Start()
    {
        // Dame hraci vsechny symptomy
        RequireSymptom<VisualDistortion>();
        RequireSymptom<Trembling>();
        RequireSymptom<HeartBeat>();
        RequireSymptom<Breathing>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        CheckIfLookingAtBlood();
        if (CurrentAnxietyLevel > 0f)
        {
            UpdateSymptoms();
        }
        else
        {
            RecoverFromSymptoms();
        }
        CurrentAnxietyLevel = 0f;
    }

    private void CheckIfLookingAtBlood()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDist,bloodMask, QueryTriggerInteraction.Collide))
        {
            PendNewAnxietyLevel(0.75f);
        }
    }
}
