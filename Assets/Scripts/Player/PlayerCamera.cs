using System;
using UnityEngine;
/// <summary>
/// Singleton kamery hráče který každý frame kalkuluje Frustumy
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    public static Camera Camera { get; private set; }
    public static Plane[] FrustumPlanes { get; private set; } = new Plane[6];

    public static float FrustumExpansionFactor { get; private set; }= 1.3f; // 10% wider on each side
    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;

    private void Start()
    {
        Camera = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        if (transform.rotation == lastCameraRotation &&
            transform.position == lastCameraPosition)
            return;
        
        lastCameraPosition = transform.position;
        lastCameraRotation = transform.rotation;
        var originalFOV = Camera.fieldOfView;
        
        Camera.fieldOfView = originalFOV * FrustumExpansionFactor;

        GeometryUtility.CalculateFrustumPlanes(Camera, FrustumPlanes);

        Camera.fieldOfView = originalFOV;
    }
}