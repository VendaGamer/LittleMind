using System.Collections;
using UnityEngine;

public class Hematophobia : AnxietyManager
{
    [SerializeField] private float maxRayDistance = 10f;
    [SerializeField] private LayerMask bloodMask;

    private Camera playerCamera;

    protected override void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        StartCoroutine(CheckIfLookingAtBlood());
    }

    /// <summary>
    /// Zkontroluje zda se hrace nekouka na krev
    /// </summary>
    private IEnumerator CheckIfLookingAtBlood()
    {
        while (true)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance,
                bloodMask, QueryTriggerInteraction.Collide))
            {
                IncreaseAnxiety(0.05f);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}