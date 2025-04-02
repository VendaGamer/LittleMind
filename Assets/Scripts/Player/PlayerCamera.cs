using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Singleton kamery hráče který každý frame kalkuluje Frustumy
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }
    public Camera Camera { get; private set; }
    public Plane[] FrustumPlanes { get; } = new Plane[6];

    [SerializeField]
    private float frustumExpansionFactor = 1.1f;

    public float FrustumExpansionFactor
    {
        get => frustumExpansionFactor;
        set
        {
            frustumExpansionFactor = value;
            UpdateFrustum();
        }
    }

    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;

    private void Start()
    {
        Camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (transform.rotation == lastCameraRotation && transform.position == lastCameraPosition)
            return;

        UpdateFrustum();
    }

    private void UpdateFrustum()
    {
        lastCameraPosition = transform.position;
        lastCameraRotation = transform.rotation;
        var originalFOV = Camera.fieldOfView;

        Camera.fieldOfView = originalFOV * FrustumExpansionFactor;

        GeometryUtility.CalculateFrustumPlanes(Camera, FrustumPlanes);

        Camera.fieldOfView = originalFOV;
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
