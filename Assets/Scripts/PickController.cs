using System;
using System.Collections;
using UnityEngine;

public abstract class PickController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform pickupPoint;
    private PickableObject holding;
    [SerializeField]
    protected Camera playerCamera;
    [SerializeField]
    private float rayDistance = 3f;
    [SerializeField]
    private float pickupLerpDuration=1f;

    protected virtual void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }
    private void PickUpObject(PickableObject obj)
    {
        holding = obj;
        holding.PickObject(pickupPoint, pickupLerpDuration);
    }
    private void DropObject()
    {
        if (holding == null) return;
        holding.DropObject();
        holding = null;
    }

    protected virtual void Update()
    {
        if (holding == null) //pickup logic
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                var gameObject = hit.collider.gameObject;
                if (gameObject == null) return;
                var pickObj = gameObject.GetComponentInParent<PickableObject>();
                if (pickObj != null)
                {
                    if (Input.GetButton("Pickup"))
                    {
                        PickUpObject(pickObj);
                    }
                }
            }
        }
        else //drop logic
        {
            if (Input.GetButton("Drop"))
            {
                DropObject();
            }
        }
    }
}
